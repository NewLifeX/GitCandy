using System.IO;
using System.Xml.Linq;
using GitCandy.Base;
using GitCandy.Data;
using GitCandy.Git;
using GitCandy.Git.Cache;
using GitCandy.Models;
using GitCandy.Web.App_GlobalResources;
using GitCandy.Web.Base;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife;
using NewLife.GitCandy.Entity;
using NewLife.Log;
using NewLife.Web;
using XCode.Membership;
using Pager = GitCandy.Base.Pager;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

public class RepositoryController : CandyControllerBase
{
    public RepositoryService RepositoryService { get; set; } = new RepositoryService();

    GitSetting _config = GitSetting.Current;
    private readonly ITracer _tracer;

    public RepositoryController(ITracer tracer) => _tracer = tracer;

    public ActionResult Index(Int32? page)
    {
        var user = Token;
        //var username = user?.Name;

        var p = new NewLife.Web.Pager
        {
            PageIndex = page ?? 1,
            PageSize = GitSetting.Current.PageSize,
            RetrieveTotalCount = true,
        };

        // 任何人只能看到自己的仓库，以及别人的公开库，即使管理员也禁止访问他人私有库
        var model = RepositoryService.GetRepositories(user as UserX, false, p);

        model.CanCreateRepository = user != null && (GitSetting.Current.AllowRepositoryCreation || user.IsAdmin());

        ViewBag.Pager = Pager.Items(model.ItemCount)
            .PerPage(GitSetting.Current.PageSize)
            .Move(model.CurrentPage)
            .Segment(5)
            .Center();

        return View(model);
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        base.OnActionExecuting(filterContext);

        var owner = filterContext.RouteData.Values["owner"] + "";
        var name = filterContext.RouteData.Values["name"] + "";
        var repo = Repository.FindByOwnerAndName(owner, name);
        if (repo != null)
            ViewBag.Title2 = repo.Description;
    }

    public ActionResult Create()
    {
        if (!_config.AllowRepositoryCreation) return Forbid();

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
        var user = UserX.FindByName(Token.Name);
        // 拥有者
        var owners = user.Teams.Where(e => e.Team != null).ToDictionary(e => e.Team.Name, e => e.TeamName);
        // 加上自己
        owners[user.Name] = user + "";
        ViewBag.Owners = owners;

        return View("Create", model);
    }

    [HttpPost]
    public ActionResult Create(RepositoryModel model)
    {
        using var span = _tracer?.NewSpan("CreateRepository", model);

        if (!_config.AllowRepositoryCreation) return Forbid();

        if (ModelState.IsValid)
        {
            try
            {
                var repo = RepositoryService.Create(model);
                if (repo != null)
                {
                    //var success = GitService.CreateRepository(model.Owner, model.Name);
                    var remoteUrl = model.HowInit.EqualIgnoreCase("Import") ? model.RemoteUrl : null;
                    var success = GitService.CreateRepository(model.Owner, model.Name, remoteUrl);
                    if (!success)
                    {
                        RepositoryService.Delete(Token?.Name, model.Name);
                        repo = null;
                    }
                }
                if (repo != null) return RedirectToAction("Tree", "Repository", new { owner = model.Owner, name = repo.Name });
            }
            catch (ArgumentException ex)
            {
                span?.SetError(ex, null);
                //ModelState.AddModelError(ex.ParamName, SR.Repository_AlreadyExists);
                ModelState.AddModelError(ex.ParamName, ex.Message);
            }
            catch (Exception ex)
            {
                span?.SetError(ex, null);
                ModelState.AddModelError("", ex.Message);
            }
        }

        return CreateView(model);
    }

    public ActionResult Detail(Int32 id)
    {
        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (!RepositoryService.CanReadRepository(repo, user)) return Forbid();

        var model = RepositoryService.Get(repo, true, user?.Name);
        if (model == null) return NotFound();

        using var git = new GitService(repo.OwnerName, repo.Name);
        model.DefaultBranch = git.GetHeadBranch();

        return View(model);
    }

    public ActionResult Edit(Int32 id)
    {
        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

        var model = RepositoryService.Get(repo, false, user?.Name);
        if (model == null) return NotFound();

        using var git = new GitService(repo.OwnerName, repo.Name);
        model.DefaultBranch = git.GetHeadBranch();
        model.LocalBranches = git.GetLocalBranches();

        return View(model);
    }

    [HttpPost]
    public ActionResult Edit(Int32 id, RepositoryModel model)
    {
        using var span = _tracer?.NewSpan("EditRepository", new { id, model });

        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (repo == null) return NotFound();

        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

        if (ModelState.IsValid)
        {
            if (!RepositoryService.Update(repo, model)) return NotFound();

            using var git = new GitService(repo.OwnerName, repo.Name);
            git.SetHeadBranch(model.DefaultBranch);

            return RedirectToAction("Detail", new { repo.ID });
        }
        else
        {
            using var git = new GitService(repo.OwnerName, repo.Name);
            model.DefaultBranch = git.GetHeadBranch();
            model.LocalBranches = git.GetLocalBranches();

            return View(model);
        }
    }

    public ActionResult Coop(Int32 id)
    {
        using var span = _tracer?.NewSpan("CoopRepository", new { id });

        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (repo == null) return NotFound();

        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

        var model = RepositoryService.GetRepositoryCollaborationModel(repo);

        return View(model);
    }

    [HttpPost]
    public ActionResult ChooseUser(Int32 id, String name, String user, String act, String value)
    {
        using var span = _tracer?.NewSpan(nameof(ChooseUser), new { id, name, user, act, value });

        String message = null;

        var repo = Repository.FindByID(id);
        var username = Token?.Name;
        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, username)) return Forbid();

        if (act == "add")
        {
            var role = RepositoryService.RepositoryAddUser(repo, user);
            if (role != null)
                return Json(new { role.AllowRead, role.AllowWrite, role.IsOwner });
        }
        else if (act == "del")
        {
            if (!Token.IsAdmin()
                 && String.Equals(user, Token?.Name, StringComparison.OrdinalIgnoreCase))
                message = SR.Account_CantRemoveSelf;
            else if (RepositoryService.RepositoryRemoveUser(repo, user))
                return Json("success");
        }
        else if (act == "read" || act == "write" || act == "owner")
        {
            var val = String.Equals(Boolean.TrueString, value, StringComparison.OrdinalIgnoreCase);
            if (!Token.IsAdmin()
                 && (act == "owner" && !val)
                 && String.Equals(user, Token?.Name, StringComparison.OrdinalIgnoreCase))
                message = SR.Account_CantRemoveSelf;
            else if (RepositoryService.RepositoryUserSetValue(repo, user, act, val))
                return Json("success");
        }

        Response.StatusCode = 400;
        return Json(message ?? SR.Shared_SomethingWrong);
    }

    [HttpPost]
    public ActionResult ChooseTeam(Int32 id, String name, String team, String act, String value)
    {
        using var span = _tracer?.NewSpan(nameof(ChooseTeam), new { id, name, team, act, value });

        var repo = Repository.FindByID(id);
        var username = Token?.Name;
        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, username)) return Forbid();

        if (act == "add")
        {
            var role = RepositoryService.RepositoryAddUser(repo, team);
            if (role != null)
                return Json(new { role.AllowRead, role.AllowWrite });
        }
        else if (act == "del")
        {
            if (RepositoryService.RepositoryRemoveUser(repo, team))
                return Json("success");
        }
        else if (act == "read" || act == "write" || act == "owner")
        {
            var val = String.Equals(Boolean.TrueString, value, StringComparison.OrdinalIgnoreCase);
            if (RepositoryService.RepositoryUserSetValue(repo, team, act, val))
                return Json("success");
        }

        Response.StatusCode = 400;
        return Json(SR.Shared_SomethingWrong);
    }

    public ActionResult Delete(Int32 id, String conform)
    {
        using var span = _tracer?.NewSpan("DeleteRepository", new { id, conform });

        if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
        {
            var repo = Repository.FindByID(id);
            var user = Token as UserX;

            if (repo == null) return NotFound();

            if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

            repo.Delete();
            //RepositoryService.Delete(owner, name);
            GitService.DeleteRepository(repo.OwnerName, repo.Name);
            GitCacheAccessor.Delete(repo.OwnerName, repo.Name);

            XTrace.WriteLine("Repository {0} deleted by {1}#{2}", repo.Name, Token?.Name, Token.ID);

            return RedirectToAction("Index");
        }

        return View((Object)id);
    }

    public ActionResult Tree(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Tree", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        var repo = Repository.FindByOwnerAndName(owner, name);

        using var git = new GitService(owner, name);
        var model = git.GetTree(path);
        if (model == null) return NotFound();

        model.Id = repo.ID;

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
                //if (model.Commit != null) repo.LastCommit = model.Commit.Committer.When.LocalDateTime;

                repo.Views++;
                repo.LastView = DateTime.Now;
                repo.Save();

                model.Description = repo.Description;
            }
        }

        // 修正Markdown
        if (model.Readme != null) model.Readme.FixMarkdown($"/{owner}/{name}");

        return View(model);
    }

    public ActionResult Blob(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Blob", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetBlob(path);
        if (model == null) return NotFound();

        // 修正Markdown
        if (model.Name.EndsWithIgnoreCase(".md")) model.FixMarkdown($"/{owner}/{name}");

        model.Name = name;

        return View(model);
    }

    public ActionResult Blame(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Blame", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetBlame(path);
        if (model == null) return NotFound();

        model.Owner = owner;
        model.Name = name;
        return View(model);
    }

    public ActionResult Raw(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Raw", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetBlob(path);
        if (model == null) return NotFound();

        return model.BlobType == BlobType.Binary
            ? new RawResult(model.RawData, FileHelper.BinaryMimeType, model.Name)
            : new RawResult(model.RawData);
    }

    public ActionResult Commit(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Commit", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetCommit(path);
        if (model == null) return NotFound();

        model.Owner = owner;
        model.Name = name;
        return View(model);
    }

    public ActionResult Compare(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Compare", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
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
                start = path[..index];
                end = path[(index + 3)..];
            }
        }
        var model = git.GetCompare(start.Replace(';', '/'), end.Replace(';', '/'));
        if (model == null) return NotFound();

        model.Owner = owner;
        model.Name = name;
        return View(model);
    }

    public ActionResult Commits(String owner, String name, String path, Int32? page)
    {
        using var span = _tracer?.NewSpan("Repository-Commits", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetCommits(path, page ?? 1, GitSetting.Current.Commits);
        if (model == null) return NotFound();

        ViewBag.Pager = Pager.Items(model.ItemCount)
            .PerPage(GitSetting.Current.Commits)
            .Move(model.CurrentPage)
            .Segment(5)
            .Center();

        model.Owner = owner;
        model.Name = name;
        return View(model);
    }

    public ActionResult Archive(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Archive", new { owner, name, path });
        try
        {
            var username = Token?.Name;
            if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

            using var git = new GitService(owner, name);
            var repo = Repository.FindByOwnerAndName(owner, name);
            if (repo != null)
            {
                repo.Downloads++;
                repo.Save();
            }

            var cacheFile = git.GetArchiveFilename(path, out var referenceName);
            if (cacheFile == null) return NotFound();

            var filename = name + "-" + referenceName;
            return File(cacheFile, "application/zip", filename + ".zip");
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
            throw;
        }
    }

    public ActionResult Tags(String owner, String name)
    {
        using var span = _tracer?.NewSpan("Repository-Tags", new { owner, name });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetTags();
        //if (model == null)
        //    throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
        model.Owner = owner;
        model.Name = name;
        model.CanDelete = Token != null && Token.IsAdmin()
            || RepositoryService.CanWriteRepository(owner, name, Token?.Name);
        return View(model);
    }

    [HttpPost]
    public ActionResult Tags(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Tags", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanWriteRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        git.DeleteTag(path);
        return Json("success");
    }

    public ActionResult Branches(String owner, String name)
    {
        using var span = _tracer?.NewSpan("Repository-Branches", new { owner, name });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetBranches();
        if (model == null) return NotFound();

        model.Owner = owner;
        model.Name = name;
        model.CanDelete = Token != null && Token.IsAdmin()
            || RepositoryService.CanWriteRepository(owner, name, Token?.Name);
        return View(model);
    }

    [HttpPost]
    public ActionResult Branches(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Branches", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanWriteRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        git.DeleteBranch(path);
        return Json("success");
    }

    public ActionResult Contributors(String owner, String name, String path)
    {
        using var span = _tracer?.NewSpan("Repository-Contributors", new { owner, name, path });

        var username = Token?.Name;
        if (!RepositoryService.CanReadRepository(owner, name, username)) return Forbid();

        using var git = new GitService(owner, name);
        var model = git.GetContributors(path);
        if (model == null) return NotFound();

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

    private GitUrl[] GetGitUrl(String owner, String name)
    {
        var url = Request.GetRawUrl();
        var path = $"/{owner}/{name}";
        var ub = new UriBuilder(url.Scheme, url.Host, url.Port, path);
        var httpUrl = ub.Uri.ToString();

        //var sshPort = UserConfiguration.Current.SshPort;
        //var sshUrl = sshPort == StartingInfo.DefaultPort
        //    ? String.Format("git@{0}:git/{1}.git", url.Host, name)
        //    : String.Format("ssh://git@{0}:{1}/git/{2}.git", url.Host, sshPort, name);

        var result = new List<GitUrl>(4)
        {
            new() { Type = url.Scheme, Url = httpUrl }
        };
        //if (UserConfiguration.Current.EnableSsh)
        //    result.Add(new GitUrl { Type = "ssh", Url = sshUrl });

        return result.ToArray();
    }

    //public Int32 sshPort { get; set; }
}