using System;
using System.Diagnostics.Contracts;
using System.Linq;
using GitCandy.Git.Cache;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class HistoryDivergenceAccessor : GitCacheAccessor<RevisionSummaryCacheItem[], HistoryDivergenceAccessor>
    {
        private readonly String key;

        public HistoryDivergenceAccessor(String repoId, Repository repo, String key)
            : base(repoId, repo)
        {
            Contract.Requires(key != null);

            this.key = key;
        }

        protected override String GetCacheKey()
        {
            return GetCacheKey(key);
        }

        protected override void Init()
        {
            var head = repo.Head;
            if (head.Tip == null)
                return;

            result = repo.Branches
                .Where(s => s != head && s.FriendlyName != "HEAD")
                .OrderByDescending(s => s.Tip.Author.When)
                .Select(branch =>
                {
                    var commit = branch.Tip;
                    return new RevisionSummaryCacheItem
                    {
                        Ahead = 0,
                        Behind = 0,
                        Name = branch.FriendlyName,
                        CommitSha = commit.Sha,
                        AuthorName = commit.Author.Name,
                        AuthorEmail = commit.Author.Email,
                        AuthorWhen = commit.Author.When,
                        CommitterName = commit.Committer.Name,
                        CommitterEmail = commit.Committer.Email,
                        CommitterWhen = commit.Committer.When,
                    };
                })
                .ToArray();
        }

        protected override void Calculate()
        {
            using (var repo = new Repository(this.repoPath))
            {
                var head = repo.Head;
                foreach (var item in result)
                {
                    var commit = repo.Branches[item.Name].Tip;
                    var divergence = repo.ObjectDatabase.CalculateHistoryDivergence(commit, head.Tip);
                    item.Ahead = divergence.AheadBy ?? 0;
                    item.Behind = divergence.BehindBy ?? 0;
                }
            }
            resultDone = true;
        }
    }
}
