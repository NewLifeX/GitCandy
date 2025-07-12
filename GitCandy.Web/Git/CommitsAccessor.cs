using GitCandy.Git.Cache;
using GitCandy.Web.Extensions;
using LibGit2Sharp;

namespace GitCandy.Git;

public class CommitsAccessor(String repoId, Repository repo, Commit commit, String path, Int32 page, Int32 pageSize) : GitCacheAccessor<RevisionSummaryCacheItem[], CommitsAccessor>(repoId, repo)
{
    public override Boolean IsAsync => false;

    protected override String GetCacheKey() => GetCacheKey(commit.Sha, path, page, pageSize);

    protected override void Init() => _result = [];

    protected override void Calculate()
    {
        using (var repo = new Repository(repoPath))
        {
            _result = repo.Commits
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
        _resultDone = true;
    }
}
