using System.ComponentModel;
using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Web;

/// <summary>账号控制器测试。覆盖用户列表、详情和搜索</summary>
[Collection("GitCandyTests")]
public class AccountControllerTests
{
    private readonly User _admin;
    private readonly User _user;

    public AccountControllerTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];

        _admin = User.Create("accadmin_" + suffix, "账号管理员", "pwd", "accadmin_" + suffix + "@test.com", null);
        _admin.IsAdmin = true;
        _admin.Update();

        _user = User.Create("accuser_" + suffix, "普通账号", "pwd", "accuser_" + suffix + "@test.com", null);
    }

    [Fact]
    [DisplayName("Index_管理员_返回用户列表")]
    public void Index_Admin_ReturnsUserList()
    {
        var controller = ControllerTestHelper.Prepare(new AccountController(), _admin);

        var result = controller.Index(null, 1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GitCandy.Models.UserListModel>(view.Model);
        Assert.True(model.ItemCount > 0);
        Assert.Contains(model.Users, e => e.Name == _user.Name);
    }

    [Fact]
    [DisplayName("Index_普通用户_拒绝访问")]
    public void Index_RegularUser_Forbids()
    {
        var controller = ControllerTestHelper.Prepare(new AccountController(), _user);

        var result = controller.Index(null, 1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    [DisplayName("Detail_存在用户_返回用户详情")]
    public void Detail_ExistingUser_ReturnsDetail()
    {
        var controller = ControllerTestHelper.Prepare(new AccountController(), _user);

        var result = controller.Detail(_user.Name);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GitCandy.Models.UserModel>(view.Model);
        Assert.Equal(_user.Name, model.Name);
        Assert.Equal(_user.NickName, model.Nickname);
    }

    [Fact]
    [DisplayName("Detail_不存在用户_返回NotFound")]
    public void Detail_NonexistentUser_ReturnsNotFound()
    {
        var controller = ControllerTestHelper.Prepare(new AccountController(), _user);

        var result = controller.Detail("missing_" + Guid.NewGuid().ToString("N"));

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    [DisplayName("Search_关键词_返回匹配用户名")]
    public void Search_Keyword_ReturnsMatchedNames()
    {
        // SQLite 的 LIKE 转义对下划线敏感，搜索用例使用纯字母数字名称
        var searchName = "accsearch" + Guid.NewGuid().ToString("N")[..6];
        User.Create(searchName, "搜索账号", "pwd", searchName + "@test.com", null);

        var controller = ControllerTestHelper.Prepare(new AccountController(), _user);

        var result = controller.Search(searchName);

        var json = Assert.IsType<JsonResult>(result);
        var names = Assert.IsType<String[]>(json.Value);
        Assert.Contains(searchName, names);
    }
}
