using System.ComponentModel;
using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GitCandyXUnitTest.Web;

/// <summary>首页控制器测试。覆盖语言切换后的跳转与 Cookie 写入</summary>
[Collection("GitCandyTests")]
public class HomeControllerTests
{
    [Fact]
    [DisplayName("Language_本站来源_跳回原页面并写入Cookie")]
    public void Language_LocalReferer_RedirectsBackAndWritesCookie()
    {
        var controller = ControllerTestHelper.Prepare(new HomeController(), null);
        controller.Request.Headers.Referer = "http://localhost/NewLife/GitCandy/Commits";

        var result = controller.Language("en-US");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/NewLife/GitCandy/Commits", redirect.Url);
        Assert.Contains("Lang=", controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    [DisplayName("Language_站外来源_跳回首页")]
    public void Language_ExternalReferer_RedirectsToStartPage()
    {
        var controller = ControllerTestHelper.Prepare(new HomeController(), null);
        controller.Request.Headers.Referer = "http://evil.example.com/phish";

        var result = controller.Language("en-US");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Repository", redirect.ControllerName);
    }
}
