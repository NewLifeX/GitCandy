using System.ComponentModel;
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
        Assert.Contains(publics, r => r.Name == "publicrepo1");
        Assert.Contains(publics, r => r.Name == "publicrepo2");
        Assert.DoesNotContain(publics, r => r.Name == "privaterepo1");
    }
}
