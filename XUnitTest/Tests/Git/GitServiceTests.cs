using System.IO.Compression;
using System.Text;
using GitCandy.Git;
using GitCandy.Web;
using LibGit2Sharp;
using Xunit;
using Xunit.Sdk;

namespace GitCandyXUnitTest.Git;

/// <summary>Git服务集成测试。使用真实LibGit2Sharp仓库测试代码浏览功能</summary>
[Collection("GitCandyTests")]
public class GitServiceTests : IDisposable
{
    private readonly String _baseDir;
    private readonly String _owner;
    private readonly String _repoName;
    private readonly String _oldRepositoryPath;
    private readonly String _oldCachePath;
    private GitService _gitService;
    private String _commitSha;
    private String? _secondCommitSha;

    public GitServiceTests()
    {
        // 注册GBK等中文编码支持
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _owner = "testowner";
        _repoName = "testrepo";
        _baseDir = Path.Combine(Path.GetTempPath(), $"GitCandyGitTest_{Guid.NewGuid():N}");

        // 配置GitSetting指向临时目录
        _oldRepositoryPath = GitSetting.Current.RepositoryPath;
        _oldCachePath = GitSetting.Current.CachePath;
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
        File.WriteAllText(readmePath, $"# Test Repository\n\nThis is a test repository. {Guid.NewGuid():N}");
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

        // 二进制文件，用于验证不解码为文本
        var binPath = Path.Combine(repo.Info.WorkingDirectory, "logo.bin");
        File.WriteAllBytes(binPath, [0x00, 0xFF, 0xFE, 0x80, 0x81, 0x00, 0xFE, 0xFF, 0x00, 0x01, 0x02, 0x03]);
        LibGit2Sharp.Commands.Stage(repo, "logo.bin");

        var commit2 = repo.Commit("添加核心代码", author, committer);
        _secondCommitSha = commit2.Sha;

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
        repo.Tags.Add("v2.0", commit2, new Signature("标签作者", "tagger@test.com", DateTimeOffset.Now), "附注标签");

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

    [Fact]
    [DisplayName("GetTree_超大说明文件_跳过预览")]
    public void GetTree_LargeReadme_SkipsPreview()
    {
        var old = GitService.MaxTextSize;
        try
        {
            GitService.MaxTextSize = 0;

            var tree = _gitService.GetTree("");

            Assert.NotNull(tree);
            Assert.NotNull(tree.Readme);
            Assert.True(tree.Readme.IsTooLarge);
            Assert.Null(tree.Readme.TextContent);
        }
        finally
        {
            GitService.MaxTextSize = old;
        }
    }

    [Fact]
    [DisplayName("GetTree_子目录_返回目录内容")]
    public void GetTree_SubDirectory_ReturnsDirectoryContent()
    {
        var tree = _gitService.GetTree("master/src");
        Assert.NotNull(tree);
        Assert.Contains(tree.Entries, e => e.Path == "src/main.cs");
        Assert.Contains(tree.Entries, e => e.Path == "src/utils.cs");
    }

    [Fact]
    [DisplayName("GetTree_注解标签_解析到标签提交")]
    public void GetTree_AnnotatedTag_ResolvesCommit()
    {
        var tree = _gitService.GetTree("v2.0");

        Assert.NotNull(tree);
        Assert.Equal("v2.0", tree.ReferenceName);
        Assert.Equal(_secondCommitSha, tree.Commit.Sha);
    }

    [Fact]
    [DisplayName("GetTree_分支名_返回分支树")]
    public void GetTree_BranchName_ReturnsTree()
    {
        var tree = _gitService.GetTree("dev");

        Assert.NotNull(tree);
        Assert.Equal("dev", tree.ReferenceName);
        Assert.Contains(tree.Entries, e => e.Path == "dev-note.md");
    }

    // ========== GIT-3: 文件内容查看 ==========

    [Fact]
    [DisplayName("GetBlob_文本文件_返回内容和编码")]
    public void GetBlob_TextFile_ReturnsContentAndEncoding()
    {
        var blob = _gitService.GetBlob(_commitSha + "/README.md");
        Assert.NotNull(blob);
        Assert.Contains("Test Repository", blob.TextContent);
    }

    [Fact]
    [DisplayName("GetBlob_代码文件_返回语法高亮")]
    public void GetBlob_CodeFile_ReturnsHighlightedContent()
    {
        var blob = _gitService.GetBlob(_commitSha + "/src/main.cs");
        Assert.NotNull(blob);
        Assert.Contains("Console.WriteLine", blob.TextContent);
        Assert.Equal("cs", blob.TextBrush);
    }

    [Fact]
    [DisplayName("GetBlob_不存在的文件_返回Null")]
    public void GetBlob_NonExistentFile_ReturnsNull()
    {
        var blob = _gitService.GetBlob(_commitSha + "/nonexistent.cs");
        Assert.Null(blob);
    }

    [Fact]
    [DisplayName("GetBlob_超大文本_跳过内容预览")]
    public void GetBlob_LargeText_SkipsPreview()
    {
        var old = GitService.MaxTextSize;
        try
        {
            // 阈值调到0，让普通 README 也触发保护逻辑
            GitService.MaxTextSize = 0;

            var blob = _gitService.GetBlob(_commitSha + "/README.md");

            Assert.NotNull(blob);
            Assert.True(blob.IsTooLarge);
            Assert.Null(blob.TextContent);
            Assert.NotEmpty(blob.RawData);
        }
        finally
        {
            GitService.MaxTextSize = old;
        }
    }

    [Fact]
    [DisplayName("GetBlob_二进制文件_不解码文本")]
    public void GetBlob_BinaryFile_SkipsTextDecoding()
    {
        var blob = _gitService.GetBlob(_commitSha + "/logo.bin");

        Assert.NotNull(blob);
        Assert.Equal(GitCandy.Models.BlobType.Binary, blob.BlobType);
        Assert.Null(blob.TextContent);
        Assert.NotEmpty(blob.RawData);
    }

    [Fact]
    [DisplayName("GetBlob_非法版本引用_返回Null")]
    public void GetBlob_InvalidRevision_ReturnsNull()
    {
        var blob = _gitService.GetBlob("notasha/README.md");

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

    [Fact]
    [DisplayName("GetCommit_分支名_返回提交详情")]
    public void GetCommit_BranchName_ReturnsDetails()
    {
        var detail = _gitService.GetCommit("master");

        Assert.NotNull(detail);
        Assert.Equal(_commitSha, detail.Sha);
    }

    [Fact]
    [DisplayName("GetCompare_空仓库_返回Null")]
    public void GetCompare_EmptyRepository_ReturnsNull()
    {
        var owner = "emptyowner";
        var name = "emptyrepo";
        using var empty = new GitService(owner, name);

        var compare = empty.GetCompare("", "");

        Assert.Null(compare);
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
    [DisplayName("GetBranches_关闭分支差异_仍返回当前分支")]
    public void GetBranches_DivergenceDisabled_StillReturnsHead()
    {
        var old = GitSetting.Current.AllowHistoryDivergence;
        try
        {
            GitSetting.Current.AllowHistoryDivergence = false;

            var branches = _gitService.GetBranches();

            Assert.NotNull(branches.Commit);
            Assert.Empty(branches.AheadBehinds);
        }
        finally
        {
            GitSetting.Current.AllowHistoryDivergence = old;
        }
    }

    [Fact]
    [DisplayName("GetHeadBranch_返回当前HEAD分支")]
    public void GetHeadBranch_ReturnsCurrentHead()
    {
        var head = _gitService.GetHeadBranch();
        Assert.Equal("master", head);
    }

    [Fact]
    [DisplayName("GetHeadBranch_切换到已有分支_更新HEAD")]
    public void SetHeadBranch_ExistingBranch_UpdatesHead()
    {
        Assert.True(_gitService.SetHeadBranch("dev"));
        Assert.Equal("dev", _gitService.GetHeadBranch());

        Assert.True(_gitService.SetHeadBranch("master"));
        Assert.Equal("master", _gitService.GetHeadBranch());
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

    [Fact]
    [DisplayName("GetTags_注解标签_返回目标提交SHA")]
    public void GetTags_AnnotatedTag_ReturnsCommitSha()
    {
        var tags = _gitService.GetTags();

        var tag = Assert.Single(tags.Tags, t => t.ReferenceName == "v2.0");
        Assert.Equal(_secondCommitSha, tag.Sha);
        Assert.Equal("附注标签", tag.MessageShort);
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

    [Fact]
    [DisplayName("GetContributors_返回贡献者统计")]
    public void GetContributors_ReturnsContributorStats()
    {
        // 贡献者统计在后台线程计算，轮询等待结果就绪
        var contributors = _gitService.GetContributors("");
        Assert.NotNull(contributors);
        Assert.NotNull(contributors.Statistics);

        var timeout = DateTime.Now.AddSeconds(10);
        while (contributors.Statistics.Current.NumberOfCommits == 0 && DateTime.Now < timeout)
        {
            Thread.Sleep(50);
            contributors = _gitService.GetContributors("");
        }
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

        var timeout = DateTime.Now.AddSeconds(10);
        while (tree.Scope.Commits == 0 && DateTime.Now < timeout)
        {
            Thread.Sleep(50);
            tree = _gitService.GetTree("");
        }

        Assert.True(tree.Scope.Commits > 0);
        Assert.True(tree.Scope.Branches > 0);
    }

    // ========== GIT-1: Git Smart HTTP传输 ==========

    [Fact]
    [DisplayName("InfoRefs_上传服务_返回引用公告")]
    public async Task InfoRefs_UploadPack_ReturnsReferences()
    {
        var core = FindGitCorePath();
        if (core == null) throw SkipException.ForSkip("未找到包含 git-upload-pack 的 git-core 目录");

        var old = GitSetting.Current.GitCorePath;
        GitSetting.Current.GitCorePath = core;
        try
        {
            using var input = new MemoryStream();
            using var output = new MemoryStream();
            await _gitService.InfoRefs("upload-pack", input, output);

            var text = Encoding.ASCII.GetString(output.ToArray());
            Assert.Contains("refs/heads/master", text);
            Assert.Contains("refs/tags/v1.0", text);
        }
        finally
        {
            GitSetting.Current.GitCorePath = old;
        }
    }

    [Fact]
    [DisplayName("InfoRefs_接收服务_返回引用公告")]
    public async Task InfoRefs_ReceivePack_ReturnsReferences()
    {
        var core = FindGitCorePath();
        if (core == null) throw SkipException.ForSkip("未找到包含 git-receive-pack 的 git-core 目录");

        var old = GitSetting.Current.GitCorePath;
        GitSetting.Current.GitCorePath = core;
        try
        {
            using var input = new MemoryStream();
            using var output = new MemoryStream();
            await _gitService.InfoRefs("receive-pack", input, output);

            var text = Encoding.ASCII.GetString(output.ToArray());
            Assert.Contains("refs/heads/master", text);
        }
        finally
        {
            GitSetting.Current.GitCorePath = old;
        }
    }

    // ========== GIT-7: 代码归档下载 ==========

    [Fact]
    [DisplayName("GetArchiveFilename_有效分支_返回Zip文件")]
    public void GetArchiveFilename_ValidBranch_ReturnsZipFile()
    {
        var file = _gitService.GetArchiveFilename("master", out var referenceName);

        Assert.NotNull(file);
        Assert.True(File.Exists(file), $"归档文件不存在：{file}，缓存目录：{GitSetting.Current.CachePath}，仓库：{_baseDir}");
        Assert.Equal("master", referenceName);

        using var zip = ZipFile.OpenRead(file);
        Assert.Contains(zip.Entries, e => e.FullName == "README.md");
        Assert.Contains(zip.Entries, e => e.FullName == "src/main.cs");
    }

    [Fact]
    [DisplayName("GetPath_路径穿越_抛出异常")]
    public void GetPath_PathTraversal_Throws()
    {
        Assert.Throws<ArgumentException>(() => GitService.GetPath("..", "repo"));
        Assert.Throws<ArgumentException>(() => GitService.GetPath("owner", "../repo"));
        Assert.Throws<ArgumentException>(() => GitService.GetPath("owner", "sub/repo"));
    }

    [Fact]
    [DisplayName("GetPath_正常名称_返回根目录下的路径")]
    public void GetPath_NormalName_ReturnsPathUnderRoot()
    {
        var path = GitService.GetPath(_owner, _repoName);

        Assert.StartsWith(Path.GetFullPath(_baseDir), path);
        Assert.EndsWith(Path.Combine(_owner, _repoName), path);
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

    // ========== 清理 ==========

    public void Dispose()
    {
        _gitService?.Dispose();

        GitSetting.Current.RepositoryPath = _oldRepositoryPath;
        GitSetting.Current.CachePath = _oldCachePath;

        try
        {
            if (Directory.Exists(_baseDir)) Directory.Delete(_baseDir, true);
        }
        catch { }
    }
}
