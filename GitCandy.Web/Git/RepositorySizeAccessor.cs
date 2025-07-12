using System;
using System.Diagnostics.Contracts;
using System.IO;
using GitCandy.Git.Cache;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class RepositorySizeAccessor : GitCacheAccessor<Int64, RepositorySizeAccessor>
    {
        private String key;

        public RepositorySizeAccessor(String repoId, Repository repo, String key)
            : base(repoId, repo)
        {
            Contract.Requires(key != null);

            this.key = key;
        }

        public override Boolean IsAsync => false;

        protected override String GetCacheKey() => GetCacheKey(key);

        protected override void Init() => _result = 0;

        protected override void Calculate()
        {
            var info = new DirectoryInfo(this.repoPath);
            foreach (var file in info.GetFiles("*", SearchOption.AllDirectories))
            {
                _result += file.Length;
            }
            _resultDone = true;
        }
    }
}
