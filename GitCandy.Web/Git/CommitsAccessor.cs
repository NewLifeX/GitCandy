using System;
using System.Diagnostics.Contracts;
using System.Linq;
using GitCandy.Git.Cache;
using GitCandy.Web.Extensions;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class CommitsAccessor : GitCacheAccessor<RevisionSummaryCacheItem[], CommitsAccessor>
    {
        private readonly Commit commit;
        private readonly String path;
        private readonly Int32 page, pageSize;

        public CommitsAccessor(String repoId, Repository repo, Commit commit, String path, Int32 page, Int32 pageSize)
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);
            Contract.Requires(path != null);
            Contract.Requires(page >= 0);
            Contract.Requires(pageSize > 0);

            this.commit = commit;
            this.path = path;
            this.page = page;
            this.pageSize = pageSize;
        }

        public override Boolean IsAsync => false;

        protected override String GetCacheKey() => GetCacheKey(commit.Sha, path, page, pageSize);

        protected override void Init() => result = new RevisionSummaryCacheItem[0];

        protected override void Calculate()
        {
            using (var repo = new Repository(this.repoPath))
            {
                result = repo.Commits
                    .QueryBy(new CommitFilter { IncludeReachableFrom = commit, SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time })
                    .PathFilter(path)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new RevisionSummaryCacheItem
                    {
                        CommitSha = s.Sha,
                        MessageShort = s.MessageShort.RepetitionIfEmpty(GitService.UnknowString),
                        AuthorName = s.Author.Name,
                        AuthorEmail = s.Author.Email,
                        AuthorWhen = s.Author.When,
                        CommitterName = s.Committer.Name,
                        CommitterEmail = s.Committer.Email,
                        CommitterWhen = s.Committer.When,
                    })
                    .ToArray();
            }
            resultDone = true;
        }
    }
}
