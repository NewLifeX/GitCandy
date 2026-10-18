using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using GitCandy.Git.Cache;
using GitCandy.Models;
using GitCandy.Web.Extensions;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class ScopeAccessor : GitCacheAccessor<RepositoryScope, ScopeAccessor>
    {
        private readonly String commitSha;
        private readonly String path;
        private readonly Boolean pathExist;

        public ScopeAccessor(String repoId, Repository repo, Commit commit, String path = "")
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);

            commitSha = commit.Sha;
            this.path = path;
            this.pathExist = commit[path] != null;
        }

        protected override String GetCacheKey() => GetCacheKey(commitSha, path);

        protected override void Init()
        {
            _result = new RepositoryScope
            {
                Commits = 0,
                Contributors = 0,
                Branches = repo.Branches.Count(),
                Tags = repo.Tags.Count(),
            };
        }

        protected override void Calculate()
        {
            using (var repo = new Repository(this.repoPath))
            {
                var tip = repo.Lookup<Commit>(commitSha);
                if (tip == null) return;

                var ancestors = pathExist
                    ? repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = tip }).PathFilter(path)
                    : repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = tip });

                var set = new HashSet<String>();
                foreach (var ancestor in ancestors)
                {
                    _result.Commits++;
                    if (set.Add(ancestor.Author.ToString()))
                        _result.Contributors++;
                }
            }
            _resultDone = true;
        }
    }
}
