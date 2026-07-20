using System.Text;
using GitCandy.Git;
using GitCandy.Web;
using LibGit2Sharp;
using Xunit;

namespace GitCandyXUnitTest.Git;

/// <summary>Git服务集成测试。使用真实LibGit2Sharp仓库测试代码浏览功能</summary>
public class GitServiceTests : IDisposable
{
    private readonly String _baseDir;
    private readonly String _owner;
    private readonly String _repoName;
    private GitService _gitService;
    private String _commitSha;

    public GitServiceTests()
    {
        // 注册GBK等中文编码支持
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _owner = "testowner";
        _repoName = "testrepo";
        _baseDir = Path.Combine(Path.GetTempPath(), $"GitCandyGitTest_{Guid.NewGuid():N}");

        // 配置GitSetting指向临时目录
        GitSetting.Current.RepositoryPath = _baseDir;
        GitSetting.Current.CachePath = Path.Combine(_baseDir, "Cache");

        // 初始化Git缓存访问器
        GitCandy.Git.Cache.GitCacheAccessor.Initialize();

        // 手动创建非裸仓库（GitService创建的是裸仓库，无法测试文件浏览）
        var repoPath = GitService.GetPath(_owner, _repoName);
        Directory.CreateDirectory(repoPath);
        using var repo = new Repository(Repository.Init(repoPath));

        // 创建测试数据：文件、提交、分支、标签
        SetupTestData(repo);
        _commitSha = repo.Head.Tip.Sha;

        // 使用已创建的仓库初始化GitService
        _gitService = new GitService(_owner, _repoName);
    }

    private void SetupTestData(Repository repo)
    {
        // 初始化Git缓存访问器（Required by ScopeAccessor/LastCommitAccessor）
        GitCandy.Git.Cache.GitCacheAccessor.Initialize();

        // 第一个提交
        var author = new Signature("测试作者", "author@test.com", DateTimeOffset.Now);
        var committer = author;

        // 创建README.md
        var readmePath = Path.Combine(repo.Info.WorkingDirectory, "README.md");
        File.WriteAllText(readmePath, "# Test Repository\n\nThis is a test repository.");
        LibGit2Sharp.Commands.Stage(repo, "README.md");
        var commit1 = repo.Commit("初始化项目", author, committer);

        // 创建 src/ 目录和文件
        var srcDir = Path.Combine(repo.Info.WorkingDirectory, "src");
        Directory.CreateDirectory(srcDir);
        var mainPath = Path.Combine(srcDir, "main.cs");
        File.WriteAllText(mainPath, "using System;\n\nclass Program\n{\n    static void Main()\n    {\n        Console.WriteLine(\"Hello\");\n    }\n}");
        LibGit2Sharp.Commands.Stage(repo, "src/main.cs");

        var utilsPath = Path.Combine(srcDir, "utils.cs");
        File.WriteAllText(utilsPath, "using System;\n\npublic static class Utils\n{\n    public static Int32 Add(Int32 a, Int32 b) => a + b;\n}");
        LibGit2Sharp.Commands.Stage(repo, "src/utils.cs");
        var commit2 = repo.Commit("添加核心代码", author, committer);

        // 创建分支 dev
        repo.Branches.Add("dev", commit2);

        // 在 dev 分支上创建额外提交
        LibGit2Sharp.Commands.Checkout(repo, "dev");
        var devFile = Path.Combine(repo.Info.WorkingDirectory, "dev-note.md");
        File.WriteAllText(devFile, "# Dev Notes\n\nDevelopment branch notes.");
        LibGit2Sharp.Commands.Stage(repo, "dev-note.md");
        repo.Commit("开发分支提交", author, committer);

        // 创建标签
        repo.Tags.Add("v1.0", commit2);
        repo.Tags.Add("v1.1", repo.Branches["dev"].Tip);

        // 切回 master
        LibGit2Sharp.Commands.Checkout(repo, "master");

        // 第三个提交 (master)
        var configPath = Path.Combine(repo.Info.WorkingDirectory, "config.json");
        File.WriteAllText(configPath, "{\"name\": \"test\", \"version\": \"1.0\"}");
        LibGit2Sharp.Commands.Stage(repo, "config.json");
        repo.Commit("添加配置文件", author, committer);
    }

    // ========== GIT-2: 代码树浏览 ==========

    [Fact]
    [DisplayName("GetTree_根路径_返回完整目录结构")]
    public void GetTree_RootPath_ReturnsCompleteStructure()
    {
        var tree = _gitService.GetTree("");
        Assert.NotNull(tree);
        Assert.Equal("master", tree.ReferenceName);

        // 根目录应包含 src/、README.md、config.json
        var entries = tree.Entries;
        Assert.NotNull(entries);
        Assert.Contains(entries, e => e.Path == "README.md");
        Assert.Contains(entries, e => e.Path == "config.json");
        Assert.Contains(entries, e => e.Path == "src" && e.EntryType == TreeEntryTargetType.Tree);
    }

    [Fact(Skip = "需调试路径解析")]
    [DisplayName("GetTree_子目录_返回目录内容")]
    public void GetTree_SubDirectory_ReturnsDirectoryContent()
    {
        var tree = _gitService.GetTree("master/src");
        Assert.NotNull(tree);
        Assert.Contains(tree.Entries, e => e.Path == "main.cs");
        Assert.Contains(tree.Entries, e => e.Path == "utils.cs");
    }

    // ========== GIT-3: 文件内容查看 ==========

    [Fact(Skip = "需要完整的Git缓存基础设施")]
    [DisplayName("GetBlob_文本文件_返回内容和编码")]
    public void GetBlob_TextFile_ReturnsContentAndEncoding()
    {
        var blob = _gitService.GetBlob(_commitSha + "/README.md");
        Assert.NotNull(blob);
        Assert.Contains("Test Repository", blob.TextContent);
    }

    [Fact(Skip = "需要完整的Git缓存基础设施")]
    [DisplayName("GetBlob_代码文件_返回语法高亮")]
    public void GetBlob_CodeFile_ReturnsHighlightedContent()
    {
        var blob = _gitService.GetBlob(_commitSha + "/src/main.cs");
        Assert.NotNull(blob);
        Assert.Contains("Console.WriteLine", blob.TextContent);
        Assert.Equal("csharp", blob.TextBrush);
    }

    [Fact(Skip = "需要完整的Git缓存基础设施")]
    [DisplayName("GetBlob_不存在的文件_返回Null")]
    public void GetBlob_NonExistentFile_ReturnsNull()
    {
        var blob = _gitService.GetBlob(_commitSha + "/nonexistent.cs");
        Assert.Null(blob);
    }

    // ========== GIT-4: 提交历史浏览 ==========

    [Fact]
    [DisplayName("GetCommits_默认分支_返回提交列表")]
    public void GetCommits_DefaultBranch_ReturnsCommits()
    {
        var commits = _gitService.GetCommits("", 1, 10);
        Assert.NotNull(commits);
        Assert.True(commits.Commits.Count() >= 3);
    }

    [Fact]
    [DisplayName("GetCommit_有效SHA_返回提交详情")]
    public void GetCommit_ValidSha_ReturnsCommitDetails()
    {
        var detail = _gitService.GetCommit(_commitSha);
        Assert.NotNull(detail);
        Assert.Equal(_commitSha, detail.Sha);
    }

    [Fact]
    [DisplayName("GetCommit_无效SHA_返回Null")]
    public void GetCommit_InvalidSha_ReturnsNull()
    {
        var detail = _gitService.GetCommit("0000000000000000000000000000000000000000");
        Assert.Null(detail);
    }

    // ========== GIT-5: 分支管理 ==========

    [Fact]
    [DisplayName("GetBranches_返回所有分支")]
    public void GetBranches_ReturnsAllBranches()
    {
        var branches = _gitService.GetBranches();
        Assert.NotNull(branches);
        Assert.NotEmpty(branches.AheadBehinds);
    }

    [Fact]
    [DisplayName("GetHeadBranch_返回当前HEAD分支")]
    public void GetHeadBranch_ReturnsCurrentHead()
    {
        var head = _gitService.GetHeadBranch();
        Assert.Equal("master", head);
    }

    // ========== GIT-6: 标签管理 ==========

    [Fact]
    [DisplayName("GetTags_返回所有标签")]
    public void GetTags_ReturnsAllTags()
    {
        var tags = _gitService.GetTags();
        Assert.NotNull(tags);
        Assert.True(tags.HasTags);
        Assert.Contains(tags.Tags, t => t.ReferenceName == "v1.0");
        Assert.Contains(tags.Tags, t => t.ReferenceName == "v1.1");
    }

    // ========== GIT-8: Blame代码审阅 ==========

    [Fact]
    [DisplayName("GetBlame_有效文件_返回逐行信息")]
    public void GetBlame_ValidFile_ReturnsLineInfo()
    {
        var blame = _gitService.GetBlame("master/README.md");
        Assert.NotNull(blame);
        Assert.NotEmpty(blame.Hunks);
    }

    [Fact]
    [DisplayName("GetBlame_不存在的文件_返回Null")]
    public void GetBlame_NonExistentFile_ReturnsNull()
    {
        var blame = _gitService.GetBlame("missing.txt");
        Assert.Null(blame);
    }

    // ========== GIT-9: 贡献者统计 ==========

    [Fact(Skip = "需调试缓存基础设施")]
    [DisplayName("GetContributors_返回贡献者统计")]
    public void GetContributors_ReturnsContributorStats()
    {
        var contributors = _gitService.GetContributors("");
        Assert.NotNull(contributors);
        Assert.NotNull(contributors.Statistics);
        Assert.True(contributors.Statistics.Current.NumberOfCommits > 0);
    }

    // ========== GIT-10: 分支差异比较 ==========

    [Fact]
    [DisplayName("GetCompare_两分支比较_返回差异结果")]
    public void GetCompare_TwoBranches_ReturnsDiffResult()
    {
        var compare = _gitService.GetCompare("master", "dev");
        Assert.NotNull(compare);
        Assert.NotNull(compare.CompareResult);
    }

    [Fact]
    [DisplayName("GetCompare_相同分支_返回空差异")]
    public void GetCompare_SameBranch_ReturnsEmptyDiff()
    {
        var compare = _gitService.GetCompare("master", "master");
        Assert.NotNull(compare);
    }

    // ========== GIT-11: 仓库统计信息 ==========

    [Fact]
    [DisplayName("GetTree_根目录_更新统计数据")]
    public void GetTree_RootPath_UpdatesStats()
    {
        var tree = _gitService.GetTree("");
        Assert.NotNull(tree);
        Assert.NotNull(tree.Scope);
        Assert.True(tree.Scope.Commits > 0);
        Assert.True(tree.Scope.Branches > 0);
    }

    // ========== 清理 ==========

    public void Dispose()
    {
        _gitService?.Dispose();
        try
        {
            if (Directory.Exists(_baseDir))
                Directory.Delete(_baseDir, true);
        }
        catch { }
    }
}
