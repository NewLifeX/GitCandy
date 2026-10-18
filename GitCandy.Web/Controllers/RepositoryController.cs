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

/// <summary>仓库控制器。提供仓库列表浏览、创建/编辑/删除、协作管理和Git代码浏览（树/文件/提交/分支/标签等）</summary>
public class RepositoryController : CandyControllerBase
{
    /// <summary>仓库服务</summary>
    public RepositoryService RepositoryService { get; set; } = new RepositoryService();

    GitSetting _config = GitSetting.Current;
    private readonly ITracer _tracer;

    /// <summary>实例化仓库控制器</summary>
    /// <param name="tracer">追踪器</param>
    public RepositoryController(ITracer tracer) => _tracer = tracer;

    /// <summary>仓库列表首页。分页显示用户可访问的仓库</summary>
    /// <param name="page">页码</param>
    /// <returns></returns>
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

        // 没有自己的仓库时，把公开库放在主列表，避免右侧出现空栏
        if (model.Collaborations == null || model.Collaborations.Length == 0)
        {
            model.Collaborations = model.Repositories ?? [];
            model.Repositories = [];
        }

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

    /// <summary>创建仓库页面</summary>
    /// <returns></returns>
    public ActionResult Create()
    {
        if (Token == null) return Forbid();
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

    /// <summary>创建仓库提交。支持从远程仓库导入</summary>
    /// <param name="model">仓库模型</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Create(RepositoryModel model)
    {
        using var span = _tracer?.NewSpan("CreateRepository", model);

        if (Token == null) return Forbid();
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

    /// <summary>仓库详情页</summary>
    /// <param name="id">仓库编号</param>
    /// <returns></returns>
    public ActionResult Detail(Int32 id)
    {
        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (!RepositoryService.CanReadRepository(repo, user)) return Forbid();

        var model = RepositoryService.Get(repo, true, user?.Name);
        if (model == null) return NotFound();

        using var git = new GitService(repo.OwnerUserName, repo.Name);
        model.DefaultBranch = git.GetHeadBranch();

        return View(model);
    }

    /// <summary>编辑仓库页面。仅管理员或仓库拥有者可访问</summary>
    /// <param name="id">仓库编号</param>
    /// <returns></returns>
    public ActionResult Edit(Int32 id)
    {
        var repo = Repository.FindByID(id);
        var user = Token as UserX;

        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

        var model = RepositoryService.Get(repo, false, user?.Name);
        if (model == null) return NotFound();

        using var git = new GitService(repo.OwnerUserName, repo.Name);
        model.DefaultBranch = git.GetHeadBranch();
        model.LocalBranches = git.GetLocalBranches();

        return View(model);
    }

    /// <summary>编辑仓库提交</summary>
    /// <param name="id">仓库编号</param>
    /// <param name="model">仓库模型</param>
    /// <returns></returns>
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

            using var git = new GitService(repo.OwnerUserName, repo.Name);
            git.SetHeadBranch(model.DefaultBranch);

            return RedirectToAction("Detail", new { repo.ID });
        }
        else
        {
            using var git = new GitService(repo.OwnerUserName, repo.Name);
            model.DefaultBranch = git.GetHeadBranch();
            model.LocalBranches = git.GetLocalBranches();

            return View(model);
        }
    }

    /// <summary>仓库协作管理页面。管理员或仓库拥有者可管理协作者</summary>
    /// <param name="id">仓库编号</param>
    /// <returns></returns>
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

    /// <summary>添加/移除用户协作者或修改权限</summary>
    /// <param name="id">仓库编号</param>
    /// <param name="name">仓库名</param>
    /// <param name="user">用户名</param>
    /// <param name="act">操作（add/del/read/write/owner）</param>
    /// <param name="value">值</param>
    /// <returns></returns>
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

    /// <summary>添加/移除团队协作者或修改权限</summary>
    /// <param name="id">仓库编号</param>
    /// <param name="name">仓库名</param>
    /// <param name="team">团队名</param>
    /// <param name="act">操作（add/del/read/write/owner）</param>
    /// <param name="value">值</param>
    /// <returns></returns>
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

    /// <summary>删除仓库。仅管理员或仓库拥有者可操作，需二次确认。同时清理Git文件和缓存</summary>
    /// <param name="id">仓库编号</param>
    /// <param name="conform">确认值（输入yes确认）</param>
    /// <returns></returns>
    public ActionResult Delete(Int32 id, String conform)
    {
        using var span = _tracer?.NewSpan("DeleteRepository", new { id, conform });

        var repo = Repository.FindByID(id);
        if (repo == null) return NotFound();

        var user = Token as UserX;
        if (!Token.IsAdmin() && !RepositoryService.IsRepositoryAdministrator(repo, user)) return Forbid();

        if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
        {
            repo.Delete();
            //RepositoryService.Delete(owner, name);
            GitService.DeleteRepository(repo.OwnerUserName, repo.Name);
            GitCacheAccessor.Delete(repo.OwnerUserName, repo.Name);

            XTrace.WriteLine("Repository {0} deleted by {1}#{2}", repo.Name, Token?.Name, Token.ID);

            return RedirectToAction("Index");
        }

        return View(repo);
    }

    /// <summary>代码树浏览。按路径查看仓库目录结构和文件列表，根目录自动刷新统计数据</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">路径</param>
    /// <returns></returns>
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

        // 空仓库没有提交，直接展示空仓库说明页；重定向到分支路径只会得到 404

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
                // 浏览根目录时顺带刷新最后提交时间，避免仓库列表显示过期数据
                if (model?.Commit != null) repo.LastCommit = model.Commit.Committer.When.LocalDateTime;

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

    /// <summary>文件内容查看（Blob视图）。支持语法高亮</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
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

        model.Owner = owner;
        model.Name = name;

        return View(model);
    }

    /// <summary>Blame代码审阅。逐行查看文件的修改历史和责任人</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
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

    /// <summary>查看文件原始内容（Raw视图）</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
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

    /// <summary>单次提交详情。查看提交信息和变更文件列表</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">提交SHA</param>
    /// <returns></returns>
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

    /// <summary>分支差异比较。比较两个分支/提交之间的代码差异</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">比较参数（start...end）</param>
    /// <returns></returns>
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

    /// <summary>提交历史列表。分页查看仓库提交记录</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">分支/路径</param>
    /// <param name="page">页码</param>
    /// <returns></returns>
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

    /// <summary>代码归档下载。按分支/标签/提交打包下载源码（zip格式）</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">分支/标签/提交</param>
    /// <returns></returns>
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
            return PhysicalFile(cacheFile, "application/zip", filename + ".zip");
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
            throw;
        }
    }

    /// <summary>查看标签列表</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <returns></returns>
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

    /// <summary>删除指定标签</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">标签名</param>
    /// <returns></returns>
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

    /// <summary>查看分支列表</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <returns></returns>
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

    /// <summary>删除指定分支</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">分支名</param>
    /// <returns></returns>
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

    /// <summary>贡献者统计。查看仓库贡献者列表及提交统计</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="path">分支/标签</param>
    /// <returns></returns>
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