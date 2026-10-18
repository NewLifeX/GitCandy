using System.ComponentModel;
using GitCandy.Web.Controllers;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Web;

/// <summary>控制器基类测试。覆盖会话用户同步与语言文化设置</summary>
[Collection("GitCandyTests")]
public class CandyControllerBaseTests
{
    public CandyControllerBaseTests(DatabaseFixture fixture) { }

    [Fact]
    [DisplayName("OnActionExecuting_会话已有用户_同步到Token")]
    public void OnActionExecuting_SessionToken_SyncsToToken()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var user = User.Create("candyuser_" + suffix, "糖果用户", "pwd", "candyuser_" + suffix + "@test.com", null);

        var controller = ControllerTestHelper.Prepare(new AccountController(), user);

        Assert.Same(user, controller.Token);
        Assert.Equal(user.Name, controller.Token.Name);
        Assert.NotNull(controller.Session);
    }

    [Fact]
    [DisplayName("OnActionExecuting_设置语言信息")]
    public void OnActionExecuting_SetsLanguageViewBag()
    {
        var controller = ControllerTestHelper.Prepare(new AccountController(), null);

        Assert.NotNull(controller.ViewBag.Language);
        Assert.NotNull(controller.ViewBag.Lang);
        Assert.Equal(0, controller.ViewBag.Identity);
    }
}
