using System.Globalization;
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
        if (!lang.IsNullOrEmpty())
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(lang);
                // CookieRequestCultureProvider 要求的格式
                Response.Cookies.Append("Lang", $"c={culture.Name}|uic={culture.Name}", new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) });
            }
            catch (CultureNotFoundException) { }
        }

        //Session["Culture"] = null;

        // 仅允许跳回本站地址，避免开放重定向
        var url = Request.GetReferer();
        if (!url.IsNullOrEmpty())
        {
            if (url.StartsWith("/") && !url.StartsWith("//")) return Redirect(url);

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && String.Equals(uri.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            {
                var port = Request.Host.Port ?? (Request.Scheme == "https" ? 443 : 80);
                if (uri.Port == port) return Redirect(uri.PathAndQuery);
            }
        }

        return RedirectToStartPage();
    }
}