using NewLife.GitCandy.Entity;
using NewLife.Data;
using Xunit;

namespace GitCandyXUnitTest.Entity;

/// <summary>Git操作日志测试</summary>
[Collection("GitCandyTests")]
public class GitHistoryTests
{
    private User _user;
    private Repository _repo;

    public GitHistoryTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _user = User.Create("ghuser_" + suffix, "日志用户", "pwd", "gh_" + suffix + "@test.com", null);
        _repo = new Repository
        {
            Name = "ghrepo_" + suffix,
            OwnerID = _user.ID,
            Enable = true,
        };
        _repo.Save();
    }

    [Fact]
    [DisplayName("Insert_有效记录_成功插入并返回Id")]
    public void Insert_ValidRecord_InsertsSuccessfully()
    {
        var history = new GitHistory
        {
            UserID = _user.ID,
            RepositoryID = _repo.ID,
            Name = _user.Name,
            Action = "git-upload-pack",
            Success = true,
            Remark = "clone",
            CreateIP = "127.0.0.1",
        };
        history.Insert();

        Assert.True(history.Id > 0);
    }

    [Fact]
    [DisplayName("FindById_存在记录_返回正确实体")]
    public void FindById_ExistingRecord_ReturnsCorrectRecord()
    {
        var history = new GitHistory
        {
            UserID = _user.ID,
            RepositoryID = _repo.ID,
            Name = _user.Name,
            Action = "git-receive-pack",
            Success = true,
            Remark = "push",
            CreateIP = "192.168.1.1",
        };
        history.Insert();

        var found = GitHistory.FindById(history.Id);
        Assert.NotNull(found);
        Assert.Equal(history.Id, found.Id);
        Assert.Equal("git-receive-pack", found.Action);
        Assert.True(found.Success);
    }

    [Fact]
    [DisplayName("FindById_不存在记录_返回Null")]
    public void FindById_NonExistingRecord_ReturnsNull()
    {
        var found = GitHistory.FindById(-999);
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("FindAllByUserIDAndAction_有效查询_返回过滤结果")]
    public void FindAllByUserIDAndAction_ValidQuery_ReturnsFilteredResults()
    {
        var action = "test-action-" + Guid.NewGuid().ToString("N")[..4];
        var history1 = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = action, Success = true };
        history1.Insert();
        var history2 = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = action, Success = false };
        history2.Insert();
        var otherAction = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = "other-action", Success = true };
        otherAction.Insert();

        var results = GitHistory.FindAllByUserIDAndAction(_user.ID, action);
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal(action, r.Action));
    }

    [Fact]
    [DisplayName("FindAllByRepositoryIDAndAction_有效查询_返回过滤结果")]
    public void FindAllByRepositoryIDAndAction_ValidQuery_ReturnsFilteredResults()
    {
        var action = "repo-action-" + Guid.NewGuid().ToString("N")[..4];
        var history = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = action, Success = true };
        history.Insert();

        var results = GitHistory.FindAllByRepositoryIDAndAction(_repo.ID, action);
        Assert.Single(results);
    }

    [Fact]
    [DisplayName("Search_多条件组合查询_正确过滤")]
    public void Search_MultipleConditions_FiltersCorrectly()
    {
        var action = "search-action-" + Guid.NewGuid().ToString("N")[..4];
        var history = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = action, Success = true, CreateIP = "10.0.0.1" };
        history.Insert();

        var param = new PageParameter { PageSize = 100 };
        var results = GitHistory.Search(_user.ID, _repo.ID, action, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.Single(results);
        Assert.Equal(action, results[0].Action);
    }

    [Fact]
    [DisplayName("Search_空条件_返回全部")]
    public void Search_EmptyConditions_ReturnsAll()
    {
        var history = new GitHistory { UserID = _user.ID, RepositoryID = _repo.ID, Name = _user.Name, Action = "empty-test", Success = true };
        history.Insert();

        // user=-1 表示不按用户过滤
        var param = new PageParameter { PageSize = 100 };
        var results = GitHistory.Search(-1, -1, null, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.NotEmpty(results);
    }
}
