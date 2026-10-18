using System.ComponentModel;
using System.Text;
using GitCandy.Data;
using GitCandy.Git;
using GitCandy.Web;
using GitCandy.Web.Controllers;
using GitCandy.Web.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using NewLife.GitCandy.Entity;
using Xunit;
using LibCommands = LibGit2Sharp.Commands;
using LibRepository = LibGit2Sharp.Repository;
using RepositoryX = NewLife.GitCandy.Entity.Repository;

namespace GitCandyXUnitTest.Web;

/// <summary>Git控制器测试。覆盖Smart HTTP入口的引用公告与权限拦截</summary>
[Collection("GitCandyTests")]
public class GitControllerTests : IDisposable
{
    private const String AuthKey = "GitCandyGitAuthorize";

    private readonly String _tempRoot;
    private readonly User _owner;
    private readonly String _repoName;
    private readonly RepositoryX _repo;
    private readonly String? _gitCore;
    private readonly String? _oldGitCore;
    private readonly String _oldRepositoryPath;
    private readonly String _oldCachePath;

    public GitControllerTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var git = GitSetting.Current;

        _gitCore = FindGitCorePath();
        _oldGitCore = git.GitCorePath;
        _oldRepositoryPath = git.RepositoryPath;
        _oldCachePath = git.CachePath;
        if (_gitCore != null) git.GitCorePath = _gitCore;

        _tempRoot = Path.Combine(Path.GetTempPath(), $"GitCandyGitCtrlTest_{suffix}");
        git.RepositoryPath = Path.Combine(_tempRoot, "Repos");
        git.CachePath = Path.Combine(_tempRoot, "Cache");

        _owner = User.Create("gitowner_" + suffix, "Git拥有者", "pwd", "gitowner_" + suffix + "@test.com", null);

        _repoName = "gitrepo_" + suffix;
        CreateGitRepository(_owner.Name, _repoName);

        var service = new RepositoryService();
        _repo = service.Create(new GitCandy.Models.RepositoryModel
        {
            Name = _repoName,
            Owner = _owner.Name,
            Description = "SmartHTTP测试仓库",
            IsPrivate = false,
            AllowAnonymousRead = true,
            AllowAnonymousWrite = false,
        });
    }

    private static void CreateGitRepository(String owner, String name)
    {
        var repoPath = GitService.GetPath(owner, name);
        Directory.CreateDirectory(repoPath);

        using var repo = new LibRepository(LibRepository.Init(repoPath));
        var author = new Signature("测试作者", "author@test.com", DateTimeOffset.Now);

        File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "README.md"), $"# Smart HTTP {Guid.NewGuid():N}");
        LibCommands.Stage(repo, "README.md");
        repo.Commit("初始化项目", author, author);
    }

    private GitController NewController(User? sessionUser)
    {
        var controller = new GitController(new AccountService(null), null);
        return ControllerTestHelper.PrepareRaw(controller, sessionUser);
    }

    [Fact]
    [DisplayName("Smart_公开仓库匿名读取_返回引用公告")]
    public async Task Smart_PublicRepoAnonymous_ReturnsAdvertisement()
    {
        if (_gitCore == null) throw Xunit.Sdk.SkipException.ForSkip("未找到包含 git-upload-pack 的 git-core 目录");

        var controller = NewController(null);
        var session = (Dictionary<String, Object?>)controller.HttpContext.Items["Session"]!;
        session[AuthKey] = null;

        var result = await controller.Smart(_owner.Name, _repoName, "git-upload-pack", "info/refs");

        Assert.IsType<EmptyResult>(result);
        var text = Encoding.ASCII.GetString(GetResponseBody(controller));
        Assert.Contains("refs/heads/master", text);
    }

    [Fact]
    [DisplayName("Smart_私有仓库匿名访问_要求认证")]
    public async Task Smart_PrivateRepoAnonymous_ReturnsUnauthorized()
    {
        _repo.IsPrivate = true;
        _repo.AllowAnonymousRead = false;
        _repo.Update();

        var controller = NewController(null);
        var session = (Dictionary<String, Object?>)controller.HttpContext.Items["Session"]!;
        session[AuthKey] = null;

        var result = await controller.Smart(_owner.Name, _repoName, "git-upload-pack", "info/refs");

        Assert.IsType<UnauthorizedResult>(result);
        Assert.Equal("Basic realm=\"GitCandy\"", controller.Response.Headers.WWWAuthenticate.ToString());
    }

    [Fact]
    [DisplayName("Smart_会话已登录拥有者_可执行推送引用公告")]
    public async Task Smart_LoggedInOwner_CanReceivePack()
    {
        if (_gitCore == null) throw Xunit.Sdk.SkipException.ForSkip("未找到包含 git-receive-pack 的 git-core 目录");

        var controller = NewController(_owner);
        var session = (Dictionary<String, Object?>)controller.HttpContext.Items["Session"]!;
        session[AuthKey] = _owner;

        var result = await controller.Smart(_owner.Name, _repoName, "git-receive-pack", "info/refs");

        Assert.IsType<EmptyResult>(result);
        var text = Encoding.ASCII.GetString(GetResponseBody(controller));
        Assert.Contains("refs/heads/master", text);
    }

    [Fact]
    [DisplayName("Smart_无服务参数_跳转代码树")]
    public async Task Smart_NoService_RedirectsToTree()
    {
        var controller = NewController(null);

        var result = await controller.Smart(_owner.Name, _repoName, null!, null!);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Tree", redirect.ActionName);
        Assert.Equal("Repository", redirect.ControllerName);
    }

    [Fact]
    [DisplayName("Smart_请求写入操作历史")]
    public async Task Smart_Request_WritesGitHistory()
    {
        var controller = NewController(null);

        await controller.Smart(_owner.Name, _repoName, null!, null!);

        var history = GitHistory.FindAll(GitHistory._.RepositoryID == _repo.ID);
        Assert.NotEmpty(history);
    }

    private static Byte[] GetResponseBody(Controller controller)
    {
        var stream = controller.Response.Body;
        stream.Position = 0;
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static String? FindGitCorePath()
    {
        var ext = OperatingSystem.IsWindows() ? ".exe" : "";
        foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            if (String.IsNullOrWhiteSpace(dir)) continue;

            try
            {
                if (File.Exists(Path.Combine(dir, "git" + ext)) &&
                    File.Exists(Path.Combine(dir, "git-upload-pack" + ext)) &&
                    File.Exists(Path.Combine(dir, "git-receive-pack" + ext)))
                    return dir;
            }
            catch { }
        }

        return null;
    }

    public void Dispose()
    {
        var git = GitSetting.Current;
        git.GitCorePath = _oldGitCore;
        git.RepositoryPath = _oldRepositoryPath;
        git.CachePath = _oldCachePath;

        try
        {
            if (Directory.Exists(_tempRoot)) Directory.Delete(_tempRoot, true);
        }
        catch { }
    }
}
