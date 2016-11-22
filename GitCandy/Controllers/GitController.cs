using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
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
        public ActionResult Smart(string project, string service, string verb)
        {
            switch (verb)
            {
                case "info/refs":
                    return InfoRefs(project, service);
                case "git-upload-pack":
                    return ExecutePack(project, "git-upload-pack");
                case "git-receive-pack":
                    return ExecutePack(project, "git-receive-pack");
                default:
                    return RedirectToAction("Tree", "Repository", new { Name = project });
            }
        }

        protected ActionResult InfoRefs(string project, string service)
        {
            Response.Charset = "";
            Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-advertisement", service);
            SetNoCache();
            Response.Write(FormatMessage(String.Format(CultureInfo.InvariantCulture, "# service={0}\n", service)));
            Response.Write(FlushMessage());

            try
            {
                using (var git = new GitService(project))
                {
                    var svc = service.Substring(4);
                    git.InfoRefs(svc, GetInputStream(), Response.OutputStream);
                }
                return new EmptyResult();
            }
            catch (RepositoryNotFoundException e)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, string.Empty, e);
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, string.Empty, e);
            }
        }

        protected ActionResult ExecutePack(string project, string service)
        {
            Response.Charset = "";
            Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-result", service);
            SetNoCache();

            try
            {
                using (var git = new GitService(project))
                {
                    var svc = service.Substring(4);
                    git.ExecutePack(svc, GetInputStream(), Response.OutputStream);

                    // 拦截提交
                    if (svc == "receive-pack")
                    {
                        // 修正提交数、分支、参与人等
                        var model = git.GetTree("");
                        var repo = NewLife.GitCandy.Entity.Repository.FindByName(project);
                        if (repo != null)
                        {
                            repo.Commits = model.Scope.Commits;
                            repo.Branches = model.Scope.Branches;
                            repo.Contributors = model.Scope.Contributors;
                            repo.LastCommit = model.Commit.Committer.When.LocalDateTime;

                            repo.Views++;
                            repo.LastView = DateTime.Now;
                            repo.SaveAsync();

                            model.Description = repo.Description;
                        }
                    }
                }
                return new EmptyResult();
            }
            catch (RepositoryNotFoundException e)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, string.Empty, e);
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, string.Empty, e);
            }
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

        private static string FormatMessage(string input)
        {
            return (input.Length + 4).ToString("X4", CultureInfo.InvariantCulture) + input;
        }

        private static string FlushMessage()
        {
            return "0000";
        }
        #endregion
    }
}
