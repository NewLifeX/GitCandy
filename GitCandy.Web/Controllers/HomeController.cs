using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;

namespace GitCandy.Web.Controllers;

/// <summary>首页控制器。提供首页跳转、关于页面和语言切换</summary>
public class HomeController : CandyControllerBase
{
    /// <summary>首页。重定向到起始页面</summary>
    /// <returns></returns>
    public ActionResult Index() => RedirectToStartPage();

    /// <summary>关于页面</summary>
    /// <returns></returns>
    [AllowAnonymous]
    public ActionResult About() => View();

    /// <summary>切换语言</summary>
    /// <param name="lang">语言代码</param>
    /// <returns></returns>
    [AllowAnonymous]
    public ActionResult Language(String lang)
    {
        Response.Cookies.Append("Lang", lang);

        //Session["Culture"] = null;

        var url = Request.GetReferer();
        return url.IsNullOrEmpty() ? RedirectToStartPage() : Redirect(url);
    }
}