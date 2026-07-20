using NewLife.GitCandy.Entity;
using NewLife.Data;
using Xunit;

namespace GitCandyXUnitTest.Entity;

/// <summary>用户仓库关联测试</summary>
[Collection("GitCandyTests")]
public class UserRepositoryTests
{
    private User _owner;
    private User _user;
    private Repository _repo;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _owner = User.Create("uro_owner_" + suffix, "拥有者", "pwd", "uro_owner_" + suffix + "@test.com", null);
        _user = User.Create("uro_user_" + suffix, "用户", "pwd", "uro_user_" + suffix + "@test.com", null);

        _repo = new Repository
        {
            Name = "uro_repo_" + suffix,
            OwnerID = _owner.ID,
            Enable = true,
        };
        _repo.Save();
    }

    private UserRepository AddUserToRepo(User user, Boolean read = true, Boolean write = false, Boolean isOwner = false)
    {
        var ur = new UserRepository
        {
            UserID = user.ID,
            RepositoryID = _repo.ID,
            AllowRead = read,
            AllowWrite = write,
            IsOwner = isOwner,
        };
        ur.Save();
        return ur;
    }

    [Fact]
    [DisplayName("FindByUserIDAndRepositoryID_存在关联_返回正确记录")]
    public void FindByUserIDAndRepositoryID_ExistingRelation_ReturnsCorrect()
    {
        AddUserToRepo(_user);
        var found = UserRepository.FindByUserIDAndRepositoryID(_user.ID, _repo.ID);
        Assert.NotNull(found);
        Assert.Equal(_user.ID, found.UserID);
        Assert.Equal(_repo.ID, found.RepositoryID);
    }

    [Fact]
    [DisplayName("FindByUserIDAndRepositoryID_不存在关联_返回Null")]
    public void FindByUserIDAndRepositoryID_NoRelation_ReturnsNull()
    {
        var found = UserRepository.FindByUserIDAndRepositoryID(-999, -999);
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("FindAllByUserID_有效用户_返回所有关联")]
    public void FindAllByUserID_ValidUser_ReturnsAllRelations()
    {
        AddUserToRepo(_user);
        var list = UserRepository.FindAllByUserID(_user.ID);
        Assert.Single(list);
        Assert.All(list, r => Assert.Equal(_user.ID, r.UserID));
    }

    [Fact]
    [DisplayName("FindAllByRepositoryID_有效仓库_返回所有关联")]
    public void FindAllByRepositoryID_ValidRepo_ReturnsAllRelations()
    {
        AddUserToRepo(_owner, isOwner: true);
        AddUserToRepo(_user);

        var list = UserRepository.FindAllByRepositoryID(_repo.ID);
        Assert.Equal(2, list.Count);
        Assert.All(list, r => Assert.Equal(_repo.ID, r.RepositoryID));
    }

    [Fact]
    [DisplayName("Search_按用户过滤_返回正确结果")]
    public void Search_ByUser_FiltersCorrectly()
    {
        AddUserToRepo(_user);
        var param = new PageParameter { PageSize = 100 };
        var results = UserRepository.Search(_user.ID, DateTime.MinValue, DateTime.MinValue, null, param);
        Assert.All(results, r => Assert.Equal(_user.ID, r.UserID));
    }
}
