using System.ComponentModel;
using NewLife.Data;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Entity;

/// <summary>仓库业务逻辑测试</summary>
[Collection("GitCandyTests")]
public class RepositoryTests
{
    private User _owner;

    public RepositoryTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _owner = User.Create("repoowner_" + suffix, "仓库拥有者", "pwd", "owner_" + suffix + "@test.com", null);
    }

    private Repository CreateTestRepo(String name, Boolean isPrivate = false)
    {
        var repo = new Repository
        {
            Name = name,
            OwnerID = _owner.ID,
            Enable = true,
            IsPrivate = isPrivate,
            AllowAnonymousRead = !isPrivate,
            Description = $"仓库{name}",
        };
        repo.Save();

        var ur = new UserRepository
        {
            UserID = _owner.ID,
            RepositoryID = repo.ID,
            IsOwner = true,
            AllowRead = true,
            AllowWrite = true,
        };
        ur.Save();

        return repo;
    }

    [Fact]
    [DisplayName("FindByOwnerAndName_存在仓库_返回正确实体")]
    public void FindByOwnerAndName_ExistingRepo_ReturnsCorrectRepo()
    {
        CreateTestRepo("testrepo1");
        var found = Repository.FindByOwnerAndName(_owner.Name, "testrepo1");
        Assert.NotNull(found);
        Assert.Equal("testrepo1", found.Name);
        Assert.Equal(_owner.ID, found.OwnerID);
    }

    [Fact]
    [DisplayName("FindByOwnerAndName_不存在仓库_返回Null")]
    public void FindByOwnerAndName_NonExistingRepo_ReturnsNull()
    {
        var found = Repository.FindByOwnerAndName("nobody", "norepo");
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("FindByID_存在仓库_返回正确实体")]
    public void FindByID_ExistingRepo_ReturnsCorrectRepo()
    {
        var repo = CreateTestRepo("findbyidtest");
        var found = Repository.FindByID(repo.ID);
        Assert.NotNull(found);
        Assert.Equal(repo.ID, found.ID);
    }

    [Fact]
    [DisplayName("FindByID_不存在仓库_返回Null")]
    public void FindByID_NonExistingRepo_ReturnsNull()
    {
        var found = Repository.FindByID(-999);
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("GetPublics_只返回公开仓库")]
    public void GetPublics_OnlyReturnsPublicRepos()
    {
        CreateTestRepo("publicrepo1", false);
        CreateTestRepo("privaterepo1", true);
        CreateTestRepo("publicrepo2", false);

        var publics = Repository.GetPublics();
        Assert.All(publics, r => Assert.False(r.IsPrivate));
        Assert.All(publics, r => Assert.True(r.AllowAnonymousRead));
        Assert.Contains(publics, r => r.Name == "publicrepo1");
        Assert.Contains(publics, r => r.Name == "publicrepo2");
        Assert.DoesNotContain(publics, r => r.Name == "privaterepo1");
    }

    [Fact]
    [DisplayName("Search_按拥有者过滤_返回正确仓库")]
    public void Search_ByOwner_FiltersCorrectly()
    {
        CreateTestRepo("search_by_owner1");
        CreateTestRepo("search_by_owner2");

        var param = new PageParameter { PageSize = 100 };
        var results = Repository.Search(_owner.ID, 0, null, null, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.All(results, r => Assert.Equal(_owner.ID, r.OwnerID));
    }

    [Fact]
    [DisplayName("Search_按启用状态过滤_正确筛选")]
    public void Search_ByEnable_FiltersCorrectly()
    {
        CreateTestRepo("enable_test1");
        var disabledRepo = new Repository
        {
            Name = "enable_test_disabled",
            OwnerID = _owner.ID,
            Enable = false,
        };
        disabledRepo.Save();

        var param = new PageParameter { PageSize = 100 };
        var enabled = Repository.Search(0, 0, true, null, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.All(enabled, r => Assert.True(r.Enable));
    }

    [Fact]
    [DisplayName("Search_按私有状态过滤_正确筛选")]
    public void Search_ByPrivate_FiltersCorrectly()
    {
        CreateTestRepo("private_filter_public", false);
        CreateTestRepo("private_filter_private", true);

        var param = new PageParameter { PageSize = 100 };
        var privates = Repository.Search(0, 0, null, true, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.All(privates, r => Assert.True(r.IsPrivate));
    }

    [Fact]
    [DisplayName("FindByOwnerIDAndName_有效参数_返回正确仓库")]
    public void FindByOwnerIDAndName_ValidParams_ReturnsCorrectRepo()
    {
        CreateTestRepo("find_by_ownerid_name");
        var found = Repository.FindByOwnerIDAndName(_owner.ID, "find_by_ownerid_name");
        Assert.NotNull(found);
        Assert.Equal(_owner.ID, found.OwnerID);
    }

    [Fact]
    [DisplayName("FindAllByOwnerID_有效拥有者_返回所有仓库")]
    public void FindAllByOwnerID_ValidOwner_ReturnsAllRepos()
    {
        CreateTestRepo("findall_owner1");
        CreateTestRepo("findall_owner2");
        CreateTestRepo("findall_owner3");

        var list = Repository.FindAllByOwnerID(_owner.ID);
        Assert.Equal(3, list.Count);
    }
}
