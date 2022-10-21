using System.Web;
using GitCandy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.Cube;
using NewLife.Model;
using NewLife.Web;
using XCode.Membership;

namespace GitCandy.Web.Controllers;

[AllowAnonymous]
public abstract class CandyControllerBase : Controller
{
    //private const String AuthKey = "_gc_auth";

    //private Token _token;

    public MembershipService MembershipService { get; set; } = new MembershipService();

    /// <summary>当前用户</summary>
    public IManageUser Token => ManageProvider.User as IManageUser;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // 如果未登录，则自动跳转登录
        var user = ManageProvider.Provider.TryLogin(HttpContext);
        if (user == null)
        {
            var url = "/Admin/User/Login";
            var returnUrl = Request.GetRawUrl();
            if (returnUrl != null) url += "?r=" + HttpUtility.UrlEncode(returnUrl.PathAndQuery);

            filterContext.Result = new RedirectResult(url);

            return;
        }
        //if (User.Identity is not IManageUser) User.Identity = user;

        // 语言文化
        var culture = Thread.CurrentThread.CurrentUICulture;
        var displayName = culture.Name.StartsWith("en")
            ? culture.NativeName
            : culture.EnglishName + " - " + culture.NativeName;

        ViewBag.Language = displayName;
        ViewBag.Lang = culture.Name;
        ViewBag.Identity = 0;

        base.OnActionExecuting(filterContext);
    }

    protected virtual ActionResult RedirectToStartPage(String returnUrl = null)
    {
        if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) return RedirectToAction("Index", "Repository");

        return Redirect(returnUrl);
    }
}