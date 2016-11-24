using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GitCandy.Data;
using GitCandy.Filters;
using GitCandy.Git;
using LibGit2Sharp;

namespace GitCandy.Controllers
{
    public class GitController : CandyControllerBase
    {
        public RepositoryService RepositoryService { get; set; } = new RepositoryService();

        [SmartGit]
        public ActionResult Smart(String owner, String project, String service, String verb)
        {
            switch (verb)
            {
                case "info/refs":
                    return InfoRefs(owner, project, service);
                case "git-upload-pack":
                    return ExecutePack(owner, project, "git-upload-pack");
                case "git-receive-pack":
                    return ExecutePack(owner, project, "git-receive-pack");
                default:
                    return RedirectToAction("Tree", "Repository", new { Owner = owner, Name = project });
            }
        }

        protected ActionResult InfoRefs(String owner, String project, String service)
        {
            Response.Charset = "";
            Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-advertisement", service);
            SetNoCache();
            Response.Write(FormatMessage(String.Format(CultureInfo.InvariantCulture, "# service={0}\n", service)));
            Response.Write(FlushMessage());

            try
            {
                using (var git = new GitService(owner, project))
                {
                    var svc = service.Substring(4);
                    git.InfoRefs(svc, GetInputStream(), Response.OutputStream);
                }
                return new EmptyResult();
            }
            catch (RepositoryNotFoundException e)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty, e);
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, String.Empty, e);
            }
        }

        protected ActionResult ExecutePack(String owner, String project, String service)
        {
            Response.Charset = "";
            Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-result", service);
            SetNoCache();

            try
            {
                using (var git = new GitService(owner, project))
                {
                    var svc = service.Substring(4);
                    git.ExecutePack(svc, GetInputStream(), Response.OutputStream);

                    // 拦截提交
                    if (svc == "receive-pack") Task.Run(() => UpdateRepo(owner, project));
                }
                return new EmptyResult();
            }
            catch (RepositoryNotFoundException e)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty, e);
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, String.Empty, e);
            }
        }

        /// <summary>更新仓库统计信息</summary>
        /// <param name="project"></param>
        private void UpdateRepo(String owner, String project)
        {
            using (var git = new GitService(owner, project))
            {
                // 修正提交数、分支、参与人等
                var commit = git.Repository.Head.Tip;
                var ancestors = git.Repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit });

                var set = new HashSet<String>();
                var cms = 0;
                var cts = 0;
                foreach (var ancestor in ancestors)
                {
                    cms++;
                    if (set.Add(ancestor.Author.ToString()))
                        cts++;
                }

                var repo = NewLife.GitCandy.Entity.Repository.FindByOwnerAndName(owner, project);
                if (repo != null)
                {
                    if (cms > 0) repo.Commits = cms;
                    repo.Branches = git.Repository.Branches.Count();
                    if (cts > 0) repo.Contributors = cts;
                    var size = 0L;
                    repo.Files = FilesInCommit(commit, out size);
                    repo.Size = size;
                    repo.LastCommit = commit.Committer.When.LocalDateTime;

                    repo.SaveAsync();
                }
            }
        }

        private int FilesInCommit(Commit commit, out long sourceSize)
        {
            var count = 0;
            var stack = new Stack<Tree>();
            sourceSize = 0;

            var repo = ((IBelongToARepository)commit).Repository;

            stack.Push(commit.Tree);
            while (stack.Count != 0)
            {
                var tree = stack.Pop();
                foreach (var entry in tree)
                    switch (entry.TargetType)
                    {
                        case TreeEntryTargetType.Blob:
                            count++;
                            sourceSize += repo.ObjectDatabase.RetrieveObjectMetadata(entry.Target.Id).Size;
                            break;
                        case TreeEntryTargetType.Tree:
                            stack.Push((Tree)entry.Target);
                            break;
                    }
            }
            return count;
        }

        #region GitController Extension
        private void SetNoCache()
        {
            Response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");
        }

        private Stream GetInputStream()
        {
            if (Request.Headers["Content-Encoding"] == "gzip")
            {
                return new GZipStream(Request.GetBufferlessInputStream(true), CompressionMode.Decompress);
            }
            return Request.GetBufferlessInputStream(true);
        }

        private static String FormatMessage(String input)
        {
            return (input.Length + 4).ToString("X4", CultureInfo.InvariantCulture) + input;
        }

        private static String FlushMessage()
        {
            return "0000";
        }
        #endregion
    }
}
