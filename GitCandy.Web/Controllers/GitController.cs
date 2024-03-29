﻿using System.Globalization;
using System.IO.Compression;
using GitCandy.Data;
using GitCandy.Git;
using GitCandy.Web.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.GitCandy.Entity;
using NewLife.Log;
using NewLife.Model;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

/// <summary>Git控制器。专用于Git客户端连接</summary>
public class GitController : Controller
{
    private const String AuthKey = "GitCandyGitAuthorize";
    private readonly AccountService _accountService;
    private readonly ITracer _tracer;

    /// <summary>临时会话扩展信息。仅限本地内存，不支持分布式共享</summary>
    public IDictionary<String, Object> Session { get; private set; }

    public RepositoryService RepositoryService { get; set; } = new RepositoryService();

    public GitController(AccountService accountService, ITracer tracer)
    {
        _accountService = accountService;
        _tracer = tracer;
    }

    [DisableRequestSizeLimit]
    public async Task<ActionResult> Smart(String owner, String project, String service, String verb)
    {
        // 进程内模拟的Session，活跃有效期20分钟
        var ctx = HttpContext;
        Session = ctx.Items["Session"] as IDictionary<String, Object>;
        var ip = HttpContext.GetUserHost();

        using var span = _tracer?.NewSpan("SmartGit", new { owner, project, service, verb });

        var history = new GitHistory
        {
            Success = true,
            Action = !service.IsNullOrEmpty() ? service : verb,
            Remark = verb,
            UserAgent = Request.Headers["User-Agent"],
            Version = Request.Headers["Git-Protocol"],
            CreateIP = ip,
        };

        var user = Session[AuthKey] as UserX;
        if (user == null)
        {
            // 从Http基本验证获取信息进行登录
            var auth = Request.Headers.Authorization.ToString();
            if (!String.IsNullOrEmpty(auth))
            {
                var certificate = auth["Basic ".Length..].ToBase64().ToStr();
                var ss = certificate.Split(':');
                var username = ss[0];
                var password = ss[1];

                history.Name = username;

                // 登录验证
                try
                {
                    user = _accountService.Login(username, password, ip);
                }
                catch (Exception ex)
                {
                    span?.SetError(ex, null);

                    XTrace.WriteLine(ex.Message);
                    //throw;

                    history.Success = false;
                    history.Remark = ex.Message;
                    history.Insert();

                    return HandleUnauthorizedRequest(user);
                }
            }
        }

        var repo = NewLife.GitCandy.Entity.Repository.FindByOwnerAndName(owner, project);

        history.UserID = user?.ID ?? 0;
        history.Name ??= user?.Name;
        history.RepositoryID = repo?.ID ?? 0;

        Session[AuthKey] = user;

        if (user == null && !GitSetting.Current.IsPublicServer)
        {
            history.Success = false;
            history.Remark = "代码库未开放";
            history.Insert();

            return HandleUnauthorizedRequest(user);
        }

        var right = false;

        if (String.IsNullOrEmpty(service)) // redirect to git browser
        {
            right = true;
        }
        else if (String.Equals(service, "git-receive-pack", StringComparison.OrdinalIgnoreCase)) // git push
        {
            right = RepositoryService.CanWriteRepository(owner, project, user?.Name);
        }
        else if (String.Equals(service, "git-upload-pack", StringComparison.OrdinalIgnoreCase)) // git fetch
        {
            right = RepositoryService.CanReadRepository(owner, project, user?.Name);
        }

        if (!right)
        {
            history.Success = false;
            history.Remark = "无权访问";
            history.Insert();

            return HandleUnauthorizedRequest(user);
        }

        try
        {
            return verb switch
            {
                "info/refs" => await InfoRefs(owner, project, service),
                "git-upload-pack" => await ExecutePack(owner, project, "git-upload-pack"),
                "git-receive-pack" => await ExecutePack(owner, project, "git-receive-pack"),
                _ => RedirectToAction("Tree", "Repository", new { Owner = owner, Name = project }),
            };
        }
        catch (Exception ex)
        {
            history.Success = false;
            history.Remark = ex.Message;

            throw;
        }
        finally
        {
            history.Insert();
        }
    }

    protected ActionResult HandleUnauthorizedRequest(IManageUser user)
    {
        if (user == null)
        {
            // 要求客户端提供基本验证的用户名和密码
            HttpContext.Response.Clear();
            HttpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"GitCandy\"";

            // 基本验证是明文传输密码，本想改为摘要验证，但是那样传输过来的是密码混合其它新的的签名，无法跟数据库对比
            //var sb = new StringBuilder();
            //sb.Append("Digest");
            //sb.Append(" realm=\"GitCandy\"");
            //sb.Append(",qop=\"auth,auth-int\"");
            //sb.Append(",nonce=\"66C4EF58DA7CB956BD04233FBB64E0A4\"");
            //filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", sb.ToString());

            //filterContext.Result = new HttpUnauthorizedResult();
            return new UnauthorizedResult();
        }
        else
        {
            throw new UnauthorizedAccessException();
        }
    }

    protected async Task<ActionResult> InfoRefs(String owner, String project, String service)
    {
        //Response.Charset = "";
        Response.ContentType = $"application/x-{service}-advertisement";

        SetNoCache();

        await Response.WriteAsync(FormatMessage($"# service={service}\n"));
        await Response.WriteAsync(FlushMessage());

        try
        {
            using (var git = new GitService(owner, project))
            {
                var svc = service[4..];
                //var buf = GetInputStream().ReadBytes();
                await git.InfoRefs(svc, GetInputStream(), Response.Body);
            }
            return new EmptyResult();
        }
        catch (RepositoryNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    protected async Task<ActionResult> ExecutePack(String owner, String project, String service)
    {
        //Response.Charset = "";
        Response.ContentType = $"application/x-{service}-result";
        SetNoCache();

        try
        {
            using (var git = new GitService(owner, project))
            {
                git.Log = XTrace.Log;
                var svc = service[4..];
                await git.ExecutePack(svc, GetInputStream(), Response.Body);

                // 拦截提交
                if (svc == "receive-pack") _ = Task.Run(() => UpdateRepo(owner, project));
            }
            return new EmptyResult();
        }
        catch (RepositoryNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>更新仓库统计信息</summary>
    /// <param name="project"></param>
    private void UpdateRepo(String owner, String project)
    {
        using var git = new GitService(owner, project);
        var cms = 0;
        var cts = 0;

        // 修正提交数、分支、参与人等
        var commit = git.Repository.Head.Tip;
        if (commit != null)
        {
            var ancestors = git.Repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit });

            var set = new HashSet<String>();
            foreach (var ancestor in ancestors)
            {
                cms++;
                if (set.Add(ancestor.Author.ToString()))
                    cts++;
            }
        }

        var repo = NewLife.GitCandy.Entity.Repository.FindByOwnerAndName(owner, project);
        if (repo != null)
        {
            if (cms > 0) repo.Commits = cms;
            repo.Branches = git.Repository.Branches.Count();
            if (cts > 0) repo.Contributors = cts;
            repo.Files = FilesInCommit(commit, out var size);
            repo.Size = size;
            repo.LastCommit = commit.Committer.When.LocalDateTime;

            repo.SaveAsync();
        }
    }

    private Int32 FilesInCommit(Commit commit, out Int64 sourceSize)
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
        //Response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
        //Response.AddHeader("Pragma", "no-cache");
        //Response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");

        Response.Headers.Expires = "Fri, 01 Jan 1980 00:00:00 GMT";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.CacheControl = "no-cache, max-age=0, must-revalidate";
    }

    private Stream GetInputStream()
    {
        if (Request.Headers["Content-Encoding"] == "gzip")
            return new GZipStream(Request.Body, CompressionMode.Decompress);

        return Request.Body;
    }

    private static String FormatMessage(String input) => (input.Length + 4).ToString("X4", CultureInfo.InvariantCulture) + input;

    private static String FlushMessage() => "0000";
    #endregion
}