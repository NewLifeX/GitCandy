using System;
using System.Diagnostics.Contracts;
using System.IO;
using GitCandy.Git.Cache;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class RepositorySizeAccessor : GitCacheAccessor<long, RepositorySizeAccessor>
    {
        private String key;

        public RepositorySizeAccessor(String repoId, Repository repo, String key)
            : base(repoId, repo)
        {
            Contract.Requires(key != null);

            this.key = key;
        }

        public override bool IsAsync { get { return false; } }

        protected override String GetCacheKey()
        {
            return GetCacheKey(key);
        }

        protected override void Init()
        {
            result = 0;
        }

        protected override void Calculate()
        {
            var info = new DirectoryInfo(this.repoPath);
            foreach (var file in info.GetFiles("*", SearchOption.AllDirectories))
            {
                result += file.Length;
            }
            resultDone = true;
        }
    }
}
