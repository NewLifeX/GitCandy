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
        private readonly Commit commit;
        private readonly String path;
        private readonly Boolean pathExist;

        public ScopeAccessor(String repoId, Repository repo, Commit commit, String path = "")
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);

            this.commit = commit;
            this.path = path;
            this.pathExist = commit[path] != null;
        }

        protected override String GetCacheKey() => GetCacheKey(commit.Sha, path);

        protected override void Init()
        {
            result = new RepositoryScope
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
                var ancestors = pathExist
                    ? repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit }).PathFilter(path)
                    : repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit });

                var set = new HashSet<String>();
                foreach (var ancestor in ancestors)
                {
                    result.Commits++;
                    if (set.Add(ancestor.Author.ToString()))
                        result.Contributors++;
                }
            }
            resultDone = true;
        }
    }
}
