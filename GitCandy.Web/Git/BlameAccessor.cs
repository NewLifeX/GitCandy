using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using GitCandy.Base;
using GitCandy.Git.Cache;
using GitCandy.Models;
using GitCandy.Web.Extensions;
using LibGit2Sharp;

namespace GitCandy.Git
{
    public class BlameAccessor : GitCacheAccessor<BlameHunkModel[], BlameAccessor>
    {
        private readonly Commit commit;
        private readonly String path, code;

        public BlameAccessor(String repoId, Repository repo, Commit commit, String path, params Encoding[] encodings)
            : base(repoId, repo)
        {
            Contract.Requires(commit != null);
            Contract.Requires(path != null);
            Contract.Requires(encodings != null);
            Contract.Requires(commit[path] != null);
            Contract.Requires(commit[path].TargetType == TreeEntryTargetType.Blob);

            var treeEntry = commit[path];

            this.commit = commit;
            this.path = path;

            var blob = (Blob)treeEntry.Target;
            var bytes = blob.GetContentStream().ToBytes();
            var encoding = FileHelper.DetectEncoding(bytes, encodings);
            this.code = FileHelper.ReadToEnd(bytes, encoding);
        }

        protected override String GetCacheKey() => GetCacheKey(commit.Sha, path);

        protected override void Init()
        {
            _result =
            [
                new() {
                    Code = code,
                    MessageShort = commit.MessageShort.RepetitionIfEmpty(GitService.UnknowString),
                    Sha = commit.Sha,
                    Author = commit.Author.Name,
                    AuthorEmail = commit.Author.Email,
                    AuthorDate = commit.Author.When,
                }
            ];
        }

        protected override void Calculate()
        {
            using (var repo = new Repository(this.repoPath))
            {
                var reader = new StringReader(code);
                var blame = repo.Blame(path, new BlameOptions { StartingAt = commit });
                _result = blame.Select(s => new BlameHunkModel
                {
                    Code = reader.ReadLines(s.LineCount),
                    MessageShort = s.FinalCommit.MessageShort.RepetitionIfEmpty(GitService.UnknowString),
                    Sha = s.FinalCommit.Sha,
                    Author = s.FinalCommit.Author.Name,
                    AuthorEmail = s.FinalCommit.Author.Email,
                    AuthorDate = s.FinalCommit.Author.When,
                })
                .ToArray();
            }
            _resultDone = true;
        }
    }
}
