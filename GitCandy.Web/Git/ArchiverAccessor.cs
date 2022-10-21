using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Text;
using GitCandy.Configuration;
using GitCandy.Git.Cache;
using LibGit2Sharp;
using NewLife;
using NewLife.Reflection;

namespace GitCandy.Git
{
    public class ArchiverAccessor : GitCacheAccessor<String, ArchiverAccessor>
    {
        private readonly Commit commit;
        private readonly Encoding[] encodings;

        public ArchiverAccessor(String repoId, Repository repo, Commit commit, params Encoding[] encodings)
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);
            Contract.Requires(encodings != null);

            this.commit = commit;
            this.encodings = encodings;
        }

        public override Boolean IsAsync => false;

        protected override String GetCacheKey() => GetCacheKey(commit.Sha);

        protected override void Init()
        {
            var info = new FileInfo(Path.Combine(UserConfiguration.Current.CachePath.GetFullPath(), GetCacheFile()));
            if (!info.Directory.Exists) info.Directory.Create();

            result = info.FullName;
        }

        protected override void Calculate()
        {
            using (var zip = ZipFile.Open(result, ZipArchiveMode.Create))
            {
                var stack = new Stack<Tree>();

                stack.Push(commit.Tree);
                while (stack.Count != 0)
                {
                    var tree = stack.Pop();
                    foreach (var entry in tree)
                    {
                        switch (entry.TargetType)
                        {
                            case TreeEntryTargetType.Blob:
                                {
                                    var zipEntry = zip.CreateEntry(entry.Path);
                                    var ms = zipEntry.Open();
                                    var blob = (Blob)entry.Target;
                                    blob.GetContentStream().CopyTo(ms);
                                    ms.Close();
                                    break;
                                }
                            case TreeEntryTargetType.Tree:
                                stack.Push((Tree)entry.Target);
                                break;
                            case TreeEntryTargetType.GitLink:
                                {
                                    var zipEntry = zip.CreateEntry(entry.Path + "/.gitsubmodule");
                                    var ms = zipEntry.Open();
                                    ms.Write(entry.Target.Sha.GetBytes());
                                    ms.Close();
                                    break;
                                }
                        }
                    }
                }
                //zip.SetComment(commit.Sha);
                var sb = new StringBuilder();
                sb.AppendLine(commit.Sha);
                sb.AppendLine(commit.Message);

                var au = commit.Author;
                if (au != null) sb.AppendFormat("{0}({1}) {2}", au.Name, au.Email, au.When.DateTime.ToFullString());

                var enc = Encoding.Default;
                zip.SetValue("_archiveComment", sb.ToString().GetBytes(enc));
            }
            resultDone = true;
        }

        protected override Boolean Load() => File.Exists(result);

        protected override void Save() { }
    }
}