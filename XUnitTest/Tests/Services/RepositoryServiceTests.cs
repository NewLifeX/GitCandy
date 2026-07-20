using System.ComponentModel;
using GitCandy.Data;
using GitCandy.Models;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Services;

/// <summary>仓库服务测试</summary>
[Collection("GitCandyTests")]
public class RepositoryServiceTests
{
    private readonly RepositoryService _service;
    private User _owner;
    private User _user;

    public RepositoryServiceTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _service = new RepositoryService();
        _owner = User.Create("repoowner_svc_" + suffix, "仓库拥有者", "pwd", "ownersvc_" + suffix + "@test.com", null);
        _user = User.Create("regularuser_" + suffix, "普通用户", "pwd", "regular_" + suffix + "@test.com", null);
    }

    private Repository CreateRepo(String name, Boolean isPrivate = false)
    {
        var model = new RepositoryModel
        {
            Name = name,
            Owner = _owner.Name,
            Description = $"测试仓库{name}",
            IsPrivate = isPrivate,
            AllowAnonymousRead = !isPrivate,
            AllowAnonymousWrite = false,
        };
        return _service.Create(model);
    }

    [Fact]
    [DisplayName("Create_有效数据_创建仓库并关联拥有者")]
    public void Create_ValidData_CreatesRepoWithOwner()
    {
        var repo = CreateRepo("create_test_repo");
        Assert.NotNull(repo);
        Assert.True(repo.ID > 0);
        Assert.Equal("create_test_repo", repo.Name);
        Assert.Equal(_owner.ID, repo.OwnerID);

        var role = UserRepository.FindByUserIDAndRepositoryID(_owner.ID, repo.ID);
        Assert.NotNull(role);
        Assert.True(role.IsOwner);
        Assert.True(role.AllowRead);
        Assert.True(role.AllowWrite);
    }

    [Fact]
    [DisplayName("Update_修改可见性_正确保存")]
    public void Update_ChangeVisibility_SavesCorrectly()
    {
        var repo = CreateRepo("visibility_test");
        Assert.False(repo.IsPrivate);

        repo.IsPrivate = true;
        var model = new RepositoryModel
        {
            Name = repo.Name,
            Owner = _owner.Name,
            IsPrivate = true,
            AllowAnonymousRead = false,
            AllowAnonymousWrite = false,
        };
        var result = _service.Update(repo, model);
        Assert.True(result);
        Assert.True(repo.IsPrivate);
    }

    [Fact]
    [DisplayName("Delete_删除仓库_清除数据库记录")]
    public void Delete_ValidRepo_DeletesRecord()
    {
        var repo = CreateRepo("delete_test_repo");
        var id = repo.ID;

        _service.Delete(_owner.Name, "delete_test_repo");

        var deleted = Repository.FindByID(id);
        Assert.Null(deleted);
    }

    [Fact]
    [DisplayName("CanReadRepository_公开仓库_匿名可读")]
    public void CanReadRepository_PublicRepo_AnonymousCanRead()
    {
        var repo = CreateRepo("public_read_test", false);
        Assert.True(_service.CanReadRepository(repo, null));
    }

    [Fact]
    [DisplayName("CanReadRepository_私有仓库_无权限用户不可读")]
    public void CanReadRepository_PrivateRepo_UnauthorizedCannotRead()
    {
        var repo = CreateRepo("private_read_test", true);
        Assert.False(_service.CanReadRepository(repo, _user));
    }

    [Fact]
    [DisplayName("IsRepositoryAdministrator_仓库拥有者_返回True")]
    public void IsRepositoryAdministrator_Owner_ReturnsTrue()
    {
        var repo = CreateRepo("admin_test_repo");
        Assert.True(_service.IsRepositoryAdministrator(repo, _owner.Name));
        Assert.True(_service.IsRepositoryAdministrator(repo, _owner));
    }

    [Fact]
    [DisplayName("IsRepositoryAdministrator_非拥有者_返回False")]
    public void IsRepositoryAdministrator_NotOwner_ReturnsFalse()
    {
        var repo = CreateRepo("not_admin_test");
        Assert.False(_service.IsRepositoryAdministrator(repo, _user.Name));
        Assert.False(_service.IsRepositoryAdministrator(repo, _user));
    }
}
