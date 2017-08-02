using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using GitCandy.Configuration;
using GitCandy.Extensions;
using LibGit2Sharp;
using NewLife.Log;
using NewLife.Threading;

namespace GitCandy.Git.Cache
{
    public abstract class GitCacheAccessor
    {
        protected static readonly Type[] accessors;
        protected static readonly object locker = new object();
        protected static readonly List<GitCacheAccessor> runningList = new List<GitCacheAccessor>();

        protected static bool enabled;

        protected Task task;

        static GitCacheAccessor()
        {
            accessors = new[]
            {
                typeof(ArchiverAccessor),
                typeof(BlameAccessor),
                typeof(CommitsAccessor),
                typeof(ContributorsAccessor),
                typeof(HistoryDivergenceAccessor),
                typeof(LastCommitAccessor),
                typeof(RepositorySizeAccessor),
                typeof(ScopeAccessor),
                typeof(SummaryAccessor),
            };
        }

        public static T Singleton<T>(T accessor) where T : GitCacheAccessor
        {
            Contract.Requires(accessor != null);

            lock (locker)
            {
                var running = runningList.OfType<T>().FirstOrDefault(s => s == accessor);
                if (running == null)
                {
                    runningList.Add(accessor);
                    accessor.Init();
                    accessor.LoadOrCalculate();
                    return accessor;
                }
                return running;
            }
        }

        private static TimerX _timer;
        public static void Initialize()
        {
            enabled = false;

            var cachePath = UserConfiguration.Current.CachePath.GetFullPath();
            if (String.IsNullOrEmpty(cachePath)) return;

            cachePath.EnsureDirectory(false);

            //var expectation = GetExpectation();
            //var reality = new String[accessors.Length];
            //var filename = Path.Combine(cachePath, "version");
            //if (File.Exists(filename))
            //{
            //    var lines = File.ReadAllLines(filename);
            //    Array.Copy(lines, reality, Math.Min(lines.Length, reality.Length));
            //}

            //for (int i = 0; i < reality.Length && i < expectation.Length; i++)
            //{
            //    if (reality[i] != expectation[i])
            //    {
            //        var path = Path.Combine(cachePath, (i + 1).ToString());
            //        if (Directory.Exists(path))
            //        {
            //            var tmpPath = path + "." + DateTime.Now.Ticks + ".del";
            //            Directory.Move(path, tmpPath);
            //        }
            //    }
            //}

            //File.WriteAllLines(filename, expectation);

            _timer = new TimerX(s =>
            {
                var dirs = Directory.GetDirectories(cachePath, "*.del");
                foreach (var dir in dirs)
                {
                    XTrace.WriteLine("Delete cache directory {0}", dir);
                    Directory.Delete(dir, true);
                }
            }, null, 10000, 10 * 60 * 1000);

            enabled = true;
        }

        //private static String[] GetExpectation()
        //{
        //    var assembly = typeof(AppInfomation).Assembly;
        //    var name = assembly.GetManifestResourceNames().Single(s => s.EndsWith(".CacheVersion"));
        //    using (var stream = assembly.GetManifestResourceStream(name))
        //    using (var reader = new StreamReader(stream))
        //    {
        //        return reader.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    }
        //}

        public static void Delete(String owner, String project)
        {
            var cachePath = UserConfiguration.Current.CachePath.GetFullPath();
            foreach (var item in accessors)
            {
                var path = cachePath.CombinePath(item.Name, owner, project);
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
        }

        protected void RemoveFromRunningPool()
        {
            lock (locker)
            {
                runningList.Remove(this);
            }
        }

        protected abstract void Init();

        protected virtual void LoadOrCalculate()
        {
            var loaded = enabled && Load();
            task = loaded
                ? new Task(() => { })
                : new Task(() =>
                {
                    try
                    {
                        Calculate();
                        if (enabled) Save();
                    }
                    catch (Exception ex)
                    {
                        XTrace.Log.Error("GitCacheAccessor {0} exception" + Environment.NewLine + "{1}", this.GetType().FullName, ex);
                    }
                });

            task.ContinueWith(t =>
            {
                Task.Delay(TimeSpan.FromMinutes(1.0)).Wait();
                RemoveFromRunningPool();
            });

            if (loaded)
            {
                task.Start();
            }
            else if (IsAsync)
            {
                //Scheduler.Instance.AddJob(new SingleJob(task));
                Task.Run(() => task.Start());
            }
            else
            {
                task.Start();
            }
        }

        protected abstract bool Load();

        protected abstract void Save();

        protected abstract void Calculate();

        public virtual bool IsAsync { get { return true; } }

        public static bool operator ==(GitCacheAccessor left, GitCacheAccessor right)
        {
            return object.ReferenceEquals(left, right)
                || !object.ReferenceEquals(left, null) && left.Equals(right)
                || !object.ReferenceEquals(right, null) && right.Equals(left);
        }

        public static bool operator !=(GitCacheAccessor left, GitCacheAccessor right)
        {
            return !object.ReferenceEquals(left, null) && !left.Equals(right)
                || !object.ReferenceEquals(right, null) && !right.Equals(left);
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException("Must override this method");
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException("Must override this method");
        }
    }

    public abstract class GitCacheAccessor<TReturn, TAccessor> : GitCacheAccessor
        where TAccessor : GitCacheAccessor<TReturn, TAccessor>
    {
        public static String Name { get; set; }
        //public static int AccessorId { get; private set; }

        protected readonly String repoId;
        protected readonly Repository repo;
        protected readonly String repoPath;

        protected TReturn result;
        protected bool resultDone;
        protected String cacheKey;

        public GitCacheReturn<TReturn> Result
        {
            get
            {
                if (task != null && !IsAsync)
                    task.Wait();
                return new GitCacheReturn<TReturn> { Value = result, Done = resultDone };
            }
        }

        static GitCacheAccessor()
        {
            Name = typeof(TAccessor).Name.TrimEnd("Accessor");
        }

        public GitCacheAccessor(String repoId, Repository repo)
        {
            Contract.Requires(repoId != null);
            Contract.Requires(repo != null);

            this.repoId = repoId;
            this.repo = repo;
            repoPath = repo.Info.Path;
        }

        protected abstract String GetCacheKey();

        protected virtual String GetCacheKey(params object[] keys)
        {
            Contract.Requires(keys != null);
            Contract.Requires(keys.Length > 0);
            Contract.Requires(keys.All(s => s != null));

            if (cacheKey != null)
                return cacheKey;

            var key = typeof(TAccessor).Name + String.Concat(keys);
            cacheKey = key.CalcSha();
            return cacheKey;
        }

        protected virtual String GetCacheFile()
        {
            return Name + "\\" + repoId + "\\" + GetCacheKey();
        }

        protected override bool Load()
        {
            var filename = Path.Combine(UserConfiguration.Current.CachePath.GetFullPath(), GetCacheFile());
            if (File.Exists(filename))
            {
                try
                {
                    using (var fs = File.Open(filename, FileMode.Open))
                    {
                        var formatter = new BinaryFormatter();
                        var value = formatter.Deserialize(fs);
                        if (value is TReturn)
                        {
                            result = (TReturn)value;
                            resultDone = true;
                            return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }

        protected override void Save()
        {
            if (!resultDone) return;

            var info = new FileInfo(Path.Combine(UserConfiguration.Current.CachePath.GetFullPath(), GetCacheFile()));
            if (!info.Directory.Exists) info.Directory.Create();

            using (var fs = info.Create())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, result);
                fs.Flush();
            }
        }

        public override bool Equals(object obj)
        {
            var accessor = obj as TAccessor;
            return accessor != null && GetCacheKey() == accessor.GetCacheKey();
        }

        public override int GetHashCode()
        {
            return typeof(TAccessor).GetHashCode() ^ GetCacheKey().GetHashCode();
        }
    }
}