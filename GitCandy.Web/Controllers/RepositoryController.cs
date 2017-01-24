using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GitCandy.Base;
using GitCandy.Configuration;
using GitCandy.Data;
using GitCandy.Filters;
using GitCandy.Git;
using GitCandy.Git.Cache;
using GitCandy.Models;
using GitCandy.Ssh;
using GitCandy.Web.App_GlobalResources;
using NewLife.GitCandy.Entity;
using NewLife.Log;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Controllers
{
    public class RepositoryController : CandyControllerBase
    {
        public RepositoryService RepositoryService { get; set; } = new RepositoryService();

        public ActionResult Index()
        {
            var username = Token?.Username;

            var p = new NewLife.Web.Pager();
            p.PageSize = 20;

            // 管理员可以看到其他人私有仓库
            var model = RepositoryService.GetRepositories(username, Token != null && Token.IsAdmin, p);

            model.CanCreateRepository = Token != null && (UserConfiguration.Current.AllowRepositoryCreation || Token.IsAdmin);

            return View(model);
        }

        [AllowRepositoryCreation]
        public ActionResult Create()
        {
            var model = new RepositoryModel
            {
                IsPrivate = false,
                AllowAnonymousRead = true,
                AllowAnonymousWrite = false,
            };

            return CreateView(model);
        }

        private ActionResult CreateView(RepositoryModel model)
        {
            var user = UserX.FindByID(Token.UserID);
            // 拥有者
            var owners = user.Teams.Where(e => e.Team != null).ToDictionary(e => e.Team.Name, e => e.TeamName);
            // 加上自己
            owners[user.Name] = user + "";
            ViewBag.Owners = owners;

            return View("Create", model);
        }

        [HttpPost]
        [AllowRepositoryCreation]
        public ActionResult Create(RepositoryModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var repo = RepositoryService.Create(model);
                    if (repo != null)
                    {
                        var success = GitService.CreateRepository(model.Owner, model.Name);
                        if (!success)
                        {
                            RepositoryService.Delete(Token.Username, model.Name);
                            repo = null;
                        }
                    }
                    if (repo != null) return RedirectToAction("Tree", "Repository", new { owner = model.Owner, name = repo.Name });
                }
                catch (ArgumentException ex)
                {
                    //ModelState.AddModelError(ex.ParamName, SR.Repository_AlreadyExists);
                    ModelState.AddModelError(ex.ParamName, ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                }
            }

            return CreateView(model);
        }

        [ReadRepository]
        public ActionResult Detail(String owner, String name)
        {
            var model = RepositoryService.Get(owner, name, true, Token?.Username);
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

            using (var git = new GitService(owner, name))
            {
                model.DefaultBranch = git.GetHeadBranch();
            }
            return View(model);
        }

        [RepositoryOwnerOrSystemAdministrator]
        public ActionResult Edit(String owner, String name)
        {
            var model = RepositoryService.Get(owner, name, username: Token.Username);
            if (model == null)
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
            using (var git = new GitService(owner, name))
            {
                model.DefaultBranch = git.GetHeadBranch();
                model.LocalBranches = git.GetLocalBranches();
            }
            return View(model);
        }

        [HttpPost]
        [RepositoryOwnerOrSystemAdministrator]
        public ActionResult Edit(String owner, String name, RepositoryModel model)
        {
            if (String.IsNullOrEmpty(name))
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

            if (ModelState.IsValid)
            {
                if (!RepositoryService.Update(owner, model))
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                using (var git = new GitService(owner, name))
                {
                    git.SetHeadBranch(model.DefaultBranch);
                }
                return RedirectToAction("Detail", new { name });
            }

            return View(model);
        }

        [RepositoryOwnerOrSystemAdministrator]
        public ActionResult Coop(String owner, String name)
        {
            if (String.IsNullOrEmpty(name))
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

            var model = RepositoryService.GetRepositoryCollaborationModel(owner, name);
            return View(model);
        }

        [HttpPost]
        [RepositoryOwnerOrSystemAdministrator]
        public JsonResult ChooseUser(String owner, String name, String user, String act, String value)
        {
            String message = null;

            if (act == "add")
            {
                var role = RepositoryService.RepositoryAddUser(owner, name, user);
                if (role != null)
                    return Json(new { role.AllowRead, role.AllowWrite, role.IsOwner });
            }
            else if (act == "del")
            {
                if (!Token.IsAdmin
                     && String.Equals(user, Token.Username, StringComparison.OrdinalIgnoreCase))
                    message = SR.Account_CantRemoveSelf;
                else if (RepositoryService.RepositoryRemoveUser(owner, name, user))
                    return Json("success");
            }
            else if (act == "read" || act == "write" || act == "owner")
            {
                var val = String.Equals(bool.TrueString, value, StringComparison.OrdinalIgnoreCase);
                if (!Token.IsAdmin
                     && (act == "owner" && !val)
                     && String.Equals(user, Token.Username, StringComparison.OrdinalIgnoreCase))
                    message = SR.Account_CantRemoveSelf;
                else if (RepositoryService.RepositoryUserSetValue(owner, name, user, act, val))
                    return Json("success");
            }

            Response.StatusCode = 400;
            return Json(message ?? SR.Shared_SomethingWrong);
        }

        [HttpPost]
        [RepositoryOwnerOrSystemAdministrator]
        public JsonResult ChooseTeam(String owner, String name, String team, String act, String value)
        {
            if (act == "add")
            {
                var role = RepositoryService.RepositoryAddUser(owner, name, team);
                if (role != null)
                    return Json(new { role.AllowRead, role.AllowWrite });
            }
            else if (act == "del")
            {
                if (RepositoryService.RepositoryRemoveUser(owner, name, team))
                    return Json("success");
            }
            else if (act == "read" || act == "write" || act == "owner")
            {
                var val = String.Equals(bool.TrueString, value, StringComparison.OrdinalIgnoreCase);
                if (RepositoryService.RepositoryUserSetValue(owner, name, team, act, val))
                    return Json("success");
            }

            Response.StatusCode = 400;
            return Json(SR.Shared_SomethingWrong);
        }

        [RepositoryOwnerOrSystemAdministrator]
        public ActionResult Delete(String owner, String name, String conform)
        {
            if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
            {
                RepositoryService.Delete(owner, name);
                GitService.DeleteRepository(owner, name);
                GitCacheAccessor.Delete(owner, name);
                XTrace.WriteLine("Repository {0} deleted by {1}#{2}", name, Token.Username, Token.UserID);
                return RedirectToAction("Index");
            }
            return View((object)name);
        }

        [ReadRepository]
        public ActionResult Tree(String owner, String name, String path)
        {
            var repo = Repository.FindByOwnerAndName(owner, name);
            using (var git = new GitService(owner, name))
            {
                var model = git.GetTree(path);
                if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

                if (model.Entries == null && model.ReferenceName != "HEAD")
                    return RedirectToAction("Tree", new { path = model.ReferenceName });

                model.GitUrls = GetGitUrl(owner, name);
                model.Owner = repo.Owner.Name;
                model.Name = name;
                if (model.IsRoot)
                {
                    //var m = RepositoryService.GetRepositoryModel(name);
                    //model.Description = m.Description;

                    // 修正提交数、分支、参与人等
                    if (repo != null)
                    {
                        var ms = model?.Scope;
                        if (ms != null && ms.Commits > 0)
                        {
                            repo.Commits = ms.Commits;
                            repo.Branches = ms.Branches;
                            repo.Contributors = ms.Contributors;
                        }
                        if (model.Commit != null) repo.LastCommit = model.Commit.Committer.When.LocalDateTime;

                        repo.Views++;
                        repo.LastView = DateTime.Now;
                        repo.Save();

                        model.Description = repo.Description;
                    }
                }
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Blob(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetBlob(path);
                if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Name = name;
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Blame(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetBlame(path);
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Owner = owner;
                model.Name = name;
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Raw(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetBlob(path);
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

                return model.BlobType == BlobType.Binary
                    ? new RawResult(model.RawData, FileHelper.BinaryMimeType, model.Name)
                    : new RawResult(model.RawData);
            }
        }

        [ReadRepository]
        public ActionResult Commit(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetCommit(path);
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Owner = owner;
                model.Name = name;
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Compare(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var start = "";
                var end = "";
                if (!String.IsNullOrEmpty(path))
                {
                    var index = path.IndexOf("...");
                    if (index == -1)
                    {
                        start = path;
                        end = "";
                    }
                    else
                    {
                        start = path.Substring(0, index);
                        end = path.Substring(index + 3);
                    }
                }
                var model = git.GetCompare(start.Replace(';', '/'), end.Replace(';', '/'));
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Owner = owner;
                model.Name = name;
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Commits(String owner, String name, String path, int? page)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetCommits(path, page ?? 1, UserConfiguration.Current.Commits);
                //if (model == null)
                //    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

                ViewBag.Pager = Pager.Items(model.ItemCount)
                    .PerPage(UserConfiguration.Current.Commits)
                    .Move(model.CurrentPage)
                    .Segment(5)
                    .Center();

                model.Owner = owner;
                model.Name = name;
                return View(model);
            }
        }

        [ReadRepository]
        public ActionResult Archive(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var repo = Repository.FindByOwnerAndName(owner, name);
                if (repo != null)
                {
                    repo.Downloads++;
                    repo.Save();
                }

                String referenceName;
                var cacheFile = git.GetArchiveFilename(path, out referenceName);
                if (cacheFile == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

                var filename = name + "-" + referenceName;
                return File(cacheFile, "application/zip", filename + ".zip");
            }
        }

        [ReadRepository]
        public ActionResult Tags(String owner, String name)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetTags();
                //if (model == null)
                //    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Owner = owner;
                model.Name = name;
                model.CanDelete = Token != null && Token.IsAdmin
                    || RepositoryService.CanWriteRepository(owner, name, Token == null ? null : Token.Username);
                return View(model);
            }
        }

        [HttpPost]
        [ReadRepository(requireWrite: true)]
        public ActionResult Tags(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                git.DeleteTag(path);
                return Json("success");
            }
        }

        [ReadRepository]
        public ActionResult Branches(String owner, String name)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetBranches();
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                model.Owner = owner;
                model.Name = name;
                model.CanDelete = Token != null && Token.IsAdmin
                    || RepositoryService.CanWriteRepository(owner, name, Token == null ? null : Token.Username);
                return View(model);
            }
        }

        [HttpPost]
        [ReadRepository(requireWrite: true)]
        public JsonResult Branches(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                git.DeleteBranch(path);
                return Json("success");
            }
        }

        [ReadRepository]
        public ActionResult Contributors(String owner, String name, String path)
        {
            using (var git = new GitService(owner, name))
            {
                var model = git.GetContributors(path);
                if (model == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);

                // 修正文件数
                var repo = Repository.FindByOwnerAndName(owner, name);
                if (repo != null)
                {
                    repo.Files = model.Statistics.Current.NumberOfFiles;
                    repo.SaveAsync();
                }

                model.Owner = owner;

                return View(model);
            }
        }

        private GitUrl[] GetGitUrl(String owner, String name)
        {
            var url = Request.Url;
            String path = VirtualPathUtility.ToAbsolute("~/" + owner + "/" + name);
            UriBuilder ub = new UriBuilder(url.Scheme, url.Host, url.Port, path);
            var httpUrl = ub.Uri.ToString();

            var sshPort = UserConfiguration.Current.SshPort;
            var sshUrl = sshPort == StartingInfo.DefaultPort
                ? String.Format("git@{0}:git/{1}.git", url.Host, name)
                : String.Format("ssh://git@{0}:{1}/git/{2}.git", url.Host, sshPort, name);

            var result = new List<GitUrl>(4);
            result.Add(new GitUrl { Type = url.Scheme, Url = httpUrl });
            if (UserConfiguration.Current.EnableSsh)
                result.Add(new GitUrl { Type = "ssh", Url = sshUrl });

            return result.ToArray();
        }

        public int sshPort { get; set; }
    }
}