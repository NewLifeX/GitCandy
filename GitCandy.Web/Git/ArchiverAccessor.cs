using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using GitCandy.Base;
using GitCandy.Configuration;
using GitCandy.Git.Cache;
using ICSharpCode.SharpZipLib.Zip;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class ArchiverAccessor : GitCacheAccessor<String, ArchiverAccessor>
    {
        private readonly Commit commit;
        private readonly Encoding[] encodings;
        private readonly String newline;

        public ArchiverAccessor(String repoId, Repository repo, Commit commit, String newline, params Encoding[] encodings)
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);
            Contract.Requires(encodings != null);

            this.commit = commit;
            this.encodings = encodings;
            this.newline = newline;
        }

        public override bool IsAsync { get { return false; } }

        protected override String GetCacheKey()
        {
            return GetCacheKey(commit.Sha, newline);
        }

        protected override void Init()
        {
            var info = new FileInfo(Path.Combine(UserConfiguration.Current.CachePath.GetFullPath(), GetCacheFile()));
            if (!info.Directory.Exists)
                info.Directory.Create();

            result = info.FullName;
        }

        protected override void Calculate()
        {
            using (var zipOutputStream = new ZipOutputStream(new FileStream(result, FileMode.Create)))
            {
                var stack = new Stack<Tree>();

                stack.Push(commit.Tree);
                while (stack.Count != 0)
                {
                    var tree = stack.Pop();
                    foreach (var entry in tree)
                    {
                        byte[] bytes;
                        switch (entry.TargetType)
                        {
                            case TreeEntryTargetType.Blob:
                                zipOutputStream.PutNextEntry(new ZipEntry(entry.Path));
                                var blob = (Blob)entry.Target;
                                bytes = blob.GetContentStream().ReadBytes();
                                if (newline == null)
                                    zipOutputStream.Write(bytes, 0, bytes.Length);
                                else
                                {
                                    var encoding = FileHelper.DetectEncoding(bytes, encodings);
                                    if (encoding == null)
                                        zipOutputStream.Write(bytes, 0, bytes.Length);
                                    else
                                    {
                                        bytes = FileHelper.ReplaceNewline(bytes, encoding, newline);
                                        zipOutputStream.Write(bytes, 0, bytes.Length);
                                    }
                                }
                                break;
                            case TreeEntryTargetType.Tree:
                                stack.Push((Tree)entry.Target);
                                break;
                            case TreeEntryTargetType.GitLink:
                                zipOutputStream.PutNextEntry(new ZipEntry(entry.Path + "/.gitsubmodule"));
                                bytes = Encoding.ASCII.GetBytes(entry.Target.Sha);
                                zipOutputStream.Write(bytes, 0, bytes.Length);
                                break;
                        }
                    }
                }
                zipOutputStream.SetComment(commit.Sha);
            }
            resultDone = true;
        }

        protected override bool Load()
        {
            return File.Exists(result);
        }

        protected override void Save()
        {
        }
    }
}
