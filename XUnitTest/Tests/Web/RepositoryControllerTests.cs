using System.ComponentModel;
using GitCandy.Data;
using GitCandy.Git;
using GitCandy.Models;
using GitCandy.Web;
using GitCandy.Web.Base;
using GitCandy.Web.Controllers;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using NewLife.GitCandy.Entity;
using Xunit;
using LibCommands = LibGit2Sharp.Commands;
using LibRepository = LibGit2Sharp.Repository;
using RepositoryX = NewLife.GitCandy.Entity.Repository;

namespace GitCandyXUnitTest.Web;

/// <summary>仓库控制器测试。使用真实Git仓库覆盖仓库浏览、协作与归档</summary>
[Collection("GitCandyTests")]
public class RepositoryControllerTests : IDisposable
{
    private readonly String _baseDir;
    private readonly String _tempRoot;
    private readonly String _repoName;
    private readonly User _owner;
    private readonly User _member;
    private readonly User _team;
    private readonly RepositoryX _repo;
    private readonly String _commitSha;
    private readonly String _oldRepositoryPath;
    private readonly String _oldCachePath;
    private readonly RepositoryService _service = new();

    public RepositoryControllerTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var git = GitSetting.Current;

        _oldRepositoryPath = git.RepositoryPath;
        _oldCachePath = git.CachePath;

        _tempRoot = Path.Combine(Path.GetTempPath(), $"GitCandyWebTest_{suffix}");
        _baseDir = Path.Combine(_tempRoot, "Repos");
        git.RepositoryPath = _baseDir;
        git.CachePath = Path.Combine(_tempRoot, "Cache");

        _owner = User.Create("webowner_" + suffix, "仓库拥有者", "pwd", "webowner_" + suffix + "@test.com", null);
        _member = User.Create("webmember_" + suffix, "仓库成员", "pwd", "webmember_" + suffix + "@test.com", null);
        _team = User.CreateTeam("webteam_" + suffix, "协作团队", "测试团队");

        _repoName = "webrepo_" + suffix;
        _commitSha = CreateGitRepository(_owner.Name, _repoName);

        _repo = _service.Create(new RepositoryModel
        {
            Name = _repoName,
            Owner = _owner.Name,
            Description = "Web控制器测试仓库",
            IsPrivate = false,
            AllowAnonymousRead = true,
            AllowAnonymousWrite = false,
        });
    }

    private String CreateGitRepository(String owner, String name)
    {
        var repoPath = GitService.GetPath(owner, name);
        Directory.CreateDirectory(repoPath);

        using var repo = new LibRepository(LibRepository.Init(repoPath));
        var author = new Signature("测试作者", "author@test.com", DateTimeOffset.Now);

        File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "README.md"), $"# Test Repository\n\nThis is a test repository. {Guid.NewGuid():N}");
        LibCommands.Stage(repo, "README.md");
        repo.Commit("初始化项目", author, author);

        var srcDir = Path.Combine(repo.Info.WorkingDirectory, "src");
        Directory.CreateDirectory(srcDir);
        File.WriteAllText(Path.Combine(srcDir, "main.cs"), "using System;\n\nclass Program\n{\n    static void Main() => Console.WriteLine(\"Hello\");\n}");
        LibCommands.Stage(repo, "src/main.cs");
        var commit = repo.Commit("添加核心代码", author, author);
        repo.Tags.Add("v1.0", commit);

        return commit.Sha;
    }

    private RepositoryController NewController(User? token) => ControllerTestHelper.Prepare(new RepositoryController(null!), token);

    [Fact]
    [DisplayName("Index_拥有者_返回仓库列表")]
    public void Index_Owner_ReturnsRepositoryList()
    {
        var controller = NewController(_owner);

        var result = controller.Index(1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RepositoryListModel>(view.Model);
        var repos = (model.Collaborations ?? []).Concat(model.Repositories ?? []).ToArray();
        Assert.Contains(repos, e => e.Name == _repoName);
        Assert.True(model.CanCreateRepository);
    }

    [Fact]
    [DisplayName("Create_页面_返回创建表单")]
    public void Create_Get_ReturnsCreateView()
    {
        var result = NewController(_owner).Create();

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<RepositoryModel>(view.Model);
    }

    [Fact]
    [DisplayName("Create_匿名用户_拒绝访问")]
    public void Create_Anonymous_Forbids()
    {
        var result = NewController(null).Create();

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    [DisplayName("Create_关闭创建但管理员_仍可打开创建页")]
    public void Create_AdminWhenCreationDisabled_ReturnsView()
    {
        var old = GitSetting.Current.AllowRepositoryCreation;
        try
        {
            GitSetting.Current.AllowRepositoryCreation = false;
            _owner.IsAdmin = true;
            _owner.Update();

            var result = NewController(_owner).Create();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<RepositoryModel>(view.Model);
        }
        finally
        {
            GitSetting.Current.AllowRepositoryCreation = old;
            _owner.IsAdmin = false;
            _owner.Update();
        }
    }

    [Fact]
    [DisplayName("Detail_公开仓库_返回详情")]
    public void Detail_PublicRepo_ReturnsDetail()
    {
        var result = NewController(_owner).Detail(_repo.ID);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RepositoryModel>(view.Model);
        Assert.Equal(_repoName, model.Name);
        Assert.Equal("master", model.DefaultBranch);
        Assert.True(model.AllowAnonymousRead);
        Assert.False(model.AllowAnonymousWrite);
    }

    [Fact]
    [DisplayName("Edit_拥有者_返回编辑页面")]
    public void Edit_Get_AsOwner_ReturnsEditView()
    {
        var result = NewController(_owner).Edit(_repo.ID);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RepositoryModel>(view.Model);
        Assert.Contains("master", model.LocalBranches);
    }

    [Fact]
    [DisplayName("Edit_提交修改_更新仓库描述")]
    public void Edit_Post_AsOwner_UpdatesRepository()
    {
        var controller = NewController(_owner);
        var model = new RepositoryModel
        {
            Name = _repoName,
            Owner = _owner.Name,
            Description = "更新后的描述",
            IsPrivate = false,
            AllowAnonymousRead = true,
            DefaultBranch = "master",
        };

        var result = controller.Edit(_repo.ID, model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Detail", redirect.ActionName);
        Assert.Equal("更新后的描述", RepositoryX.FindByID(_repo.ID).Description);
    }

    [Fact]
    [DisplayName("Coop_拥有者_返回协作模型")]
    public void Coop_AsOwner_ReturnsCollaborationModel()
    {
        var result = NewController(_owner).Coop(_repo.ID);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CollaborationModel>(view.Model);
        Assert.Equal(_repo.ID, model.Id);
        Assert.Contains(model.Users, e => e.Name == _owner.Name && e.IsOwner);
    }

    [Fact]
    [DisplayName("ChooseUser_添加修改删除_全部生效")]
    public void ChooseUser_AddUpdateDelete_AllWork()
    {
        var controller = NewController(_owner);

        var add = controller.ChooseUser(_repo.ID, _repoName, _member.Name, "add", null);
        Assert.IsType<JsonResult>(add);
        var role = UserRepository.FindByUserIDAndRepositoryID(_member.ID, _repo.ID);
        Assert.NotNull(role);

        var write = controller.ChooseUser(_repo.ID, _repoName, _member.Name, "write", "true");
        Assert.Equal("success", Assert.IsType<JsonResult>(write).Value);
        Assert.True(UserRepository.FindByUserIDAndRepositoryID(_member.ID, _repo.ID).AllowWrite);

        var del = controller.ChooseUser(_repo.ID, _repoName, _member.Name, "del", null);
        Assert.Equal("success", Assert.IsType<JsonResult>(del).Value);
        Assert.Null(UserRepository.FindByUserIDAndRepositoryID(_member.ID, _repo.ID));
    }

    [Fact]
    [DisplayName("ChooseTeam_添加删除_全部生效")]
    public void ChooseTeam_AddDelete_AllWork()
    {
        var controller = NewController(_owner);

        var add = controller.ChooseTeam(_repo.ID, _repoName, _team.Name, "add", null);
        Assert.IsType<JsonResult>(add);
        Assert.NotNull(UserRepository.FindByUserIDAndRepositoryID(_team.ID, _repo.ID));

        var del = controller.ChooseTeam(_repo.ID, _repoName, _team.Name, "del", null);
        Assert.Equal("success", Assert.IsType<JsonResult>(del).Value);
        Assert.Null(UserRepository.FindByUserIDAndRepositoryID(_team.ID, _repo.ID));
    }

    [Fact]
    [DisplayName("Tree_根目录_返回目录条目和README")]
    public void Tree_Root_ReturnsEntriesAndReadme()
    {
        var result = NewController(_owner).Tree(_owner.Name, _repoName, null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TreeModel>(view.Model);
        Assert.True(model.IsRoot);
        Assert.Contains(model.Entries, e => e.Path == "README.md");
        Assert.Contains(model.Entries, e => e.Path == "src");
        Assert.NotNull(model.Readme);
        Assert.Contains("Test Repository", model.Readme.TextContent);
    }

    [Fact]
    [DisplayName("Blob_代码文件_返回高亮内容")]
    public void Blob_CodeFile_ReturnsHighlightedContent()
    {
        var result = NewController(_owner).Blob(_owner.Name, _repoName, "master/src/main.cs");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TreeEntryModel>(view.Model);
        Assert.Contains("Console.WriteLine", model.TextContent);
        Assert.Equal("cs", model.TextBrush);
        Assert.Equal(_owner.Name, model.Owner);
        Assert.Equal(_repoName, model.Name);
        Assert.Equal("main.cs", model.FileName);
    }

    [Fact]
    [DisplayName("简洁地址_查询串路径_回填到动作参数")]
    public void OnActionExecuting_QueryPath_FillsEmptyArgument()
    {
        var controller = new RepositoryController(null!);
        var args = new Dictionary<String, Object?>
        {
            ["owner"] = _owner.Name,
            ["name"] = _repoName,
            ["path"] = null,
        };

        ControllerTestHelper.ExecuteFilters(controller, _owner, "?path=master/src", args);
        Assert.Equal("master/src", args["path"]);

        // 路由段已提供路径时不覆盖
        var routeArgs = new Dictionary<String, Object?>
        {
            ["owner"] = _owner.Name,
            ["name"] = _repoName,
            ["path"] = "master/Doc",
        };
        ControllerTestHelper.ExecuteFilters(controller, _owner, "?path=master/src", routeArgs);
        Assert.Equal("master/Doc", routeArgs["path"]);
    }

    [Fact]
    [DisplayName("Blob_目录路径_重定向到目录浏览")]
    public void Blob_DirectoryPath_RedirectsToTree()    {
        var result = NewController(_owner).Blob(_owner.Name, _repoName, "master/src");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Contains("/Tree/", redirect.Url);
        Assert.EndsWith("master/src", redirect.Url);
    }

    [Fact]
    [DisplayName("Blob_不存在的文件_返回404")]
    public void Blob_NonExistentFile_ReturnsNotFound()
    {
        var result = NewController(_owner).Blob(_owner.Name, _repoName, "master/nope.txt");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [DisplayName("Raw_文本文件_返回原始内容")]
    public void Raw_TextFile_ReturnsRawContent()
    {
        var result = NewController(_owner).Raw(_owner.Name, _repoName, "master/README.md");

        var raw = Assert.IsType<RawResult>(result);
        Assert.Contains("Test Repository", System.Text.Encoding.UTF8.GetString(raw.Contents));
        // 文本内容需要声明字符集，避免浏览器按系统编码解析导致中文乱码
        Assert.StartsWith("text/plain", raw.ContentType);
        Assert.Contains("charset=", raw.ContentType);
    }

    [Fact]
    [DisplayName("Blame_有效文件_返回逐行信息")]
    public void Blame_ValidFile_ReturnsHunks()
    {
        var result = NewController(_owner).Blame(_owner.Name, _repoName, "master/README.md");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BlameModel>(view.Model);
        Assert.NotEmpty(model.Hunks);
    }

    [Fact]
    [DisplayName("Commit_有效提交_返回变更文件")]
    public void Commit_ValidSha_ReturnsChanges()
    {
        var result = NewController(_owner).Commit(_owner.Name, _repoName, _commitSha);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CommitModel>(view.Model);
        Assert.Equal(_commitSha, model.Sha);
        Assert.NotNull(model.Changes);
    }

    [Fact]
    [DisplayName("Commits_默认分支_返回提交列表")]
    public void Commits_DefaultBranch_ReturnsList()
    {
        var result = NewController(_owner).Commits(_owner.Name, _repoName, null, 1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CommitsModel>(view.Model);
        Assert.True(model.Commits.Count() >= 2);
        Assert.NotEmpty(model.Commits);
    }

    [Fact]
    [DisplayName("Branches_返回分支列表")]
    public void Branches_ReturnsBranchList()
    {
        var result = NewController(_owner).Branches(_owner.Name, _repoName);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BranchesModel>(view.Model);
        Assert.Equal("master", model.Commit.ReferenceName);
        Assert.DoesNotContain(model.AheadBehinds, e => e.Commit.ReferenceName == "master");
    }

    [Fact]
    [DisplayName("Tags_返回标签列表")]
    public void Tags_ReturnsTagList()
    {
        var result = NewController(_owner).Tags(_owner.Name, _repoName);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TagsModel>(view.Model);
        Assert.Contains(model.Tags, e => e.ReferenceName == "v1.0");
    }

    [Fact]
    [DisplayName("Compare_相同分支_返回空差异")]
    public void Compare_SameBranch_ReturnsEmptyDiff()
    {
        var result = NewController(_owner).Compare(_owner.Name, _repoName, "master...master");

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<CompareModel>(view.Model);
    }

    [Fact]
    [DisplayName("Archive_有效分支_返回Zip文件")]
    public void Archive_ValidBranch_ReturnsZip()
    {
        var result = NewController(_owner).Archive(_owner.Name, _repoName, "master");

        var file = Assert.IsType<PhysicalFileResult>(result);
        Assert.Equal("application/zip", file.ContentType);
        Assert.EndsWith(".zip", file.FileDownloadName);
        Assert.True(File.Exists(file.FileName));
    }

    [Fact]
    [DisplayName("Contributors_返回贡献者统计")]
    public void Contributors_ReturnsStatistics()
    {
        var controller = NewController(_owner);

        var result = controller.Contributors(_owner.Name, _repoName, null);
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ContributorsModel>(view.Model);
        Assert.NotNull(model.Statistics);

        // 贡献者统计在后台线程计算，轮询等待结果就绪
        var timeout = DateTime.Now.AddSeconds(10);
        while (model.Statistics.Current.NumberOfCommits == 0 && DateTime.Now < timeout)
        {
            Thread.Sleep(50);
            result = controller.Contributors(_owner.Name, _repoName, null);
            model = Assert.IsType<ContributorsModel>(Assert.IsType<ViewResult>(result).Model);
        }
        Assert.True(model.Statistics.Current.NumberOfCommits > 0);
    }

    [Fact]
    [DisplayName("Tree_私有仓库匿名用户_拒绝访问")]
    public void Tree_PrivateRepoAnonymous_Forbids()
    {
        _repo.IsPrivate = true;
        _repo.AllowAnonymousRead = false;
        _repo.Update();

        var result = NewController(null).Tree(_owner.Name, _repoName, null);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    [DisplayName("Delete_拥有者_返回仓库确认页")]
    public void Delete_Get_Owner_ReturnsRepositoryView()
    {
        var result = NewController(_owner).Delete(_repo.ID, null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RepositoryX>(view.Model);
        Assert.Equal(_repo.ID, model.ID);
        Assert.Equal(_repoName, model.Name);
    }

    [Fact]
    [DisplayName("Delete_非管理员_拒绝访问")]
    public void Delete_Get_NonAdmin_Forbids()
    {
        var result = NewController(_member).Delete(_repo.ID, null);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    [DisplayName("Tree_空仓库_返回空仓库说明页")]
    public void Tree_EmptyRepository_ReturnsEmptyView()
    {
        var name = "emptyrepo_" + Guid.NewGuid().ToString("N")[..6];
        var repoPath = GitService.GetPath(_owner.Name, name);
        Directory.CreateDirectory(repoPath);
        LibRepository.Init(repoPath);

        _service.Create(new RepositoryModel
        {
            Name = name,
            Owner = _owner.Name,
            Description = "空仓库",
            IsPrivate = false,
            AllowAnonymousRead = true,
        });

        var result = NewController(_owner).Tree(_owner.Name, name, null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TreeModel>(view.Model);
        Assert.Null(model.Entries);
        Assert.NotNull(model.ReferenceName);
    }

    public void Dispose()
    {
        var git = GitSetting.Current;
        git.RepositoryPath = _oldRepositoryPath;
        git.CachePath = _oldCachePath;

        try
        {
            if (Directory.Exists(_tempRoot)) Directory.Delete(_tempRoot, true);
        }
        catch { }
    }
}
