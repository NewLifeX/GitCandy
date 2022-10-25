using GitCandy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.Cube;
using NewLife.Model;
using XCode.Membership;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

[AllowAnonymous]
public abstract class CandyControllerBase : Controller
{
    //private const String AuthKey = "_gc_auth";

    //private Token _token;

    /// <summary>临时会话扩展信息。仅限本地内存，不支持分布式共享</summary>
    public IDictionary<String, Object> Session { get; private set; }

    /// <summary>用户主机</summary>
    public String UserHost { get; private set; }

    public MembershipService MembershipService { get; set; } = new MembershipService();

    /// <summary>当前用户</summary>
    public IManageUser Token { get; private set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // 进程内模拟的Session，活跃有效期20分钟
        var ctx = filterContext.HttpContext;
        Session = ctx.Items["Session"] as IDictionary<String, Object>;

        UserHost = HttpContext.GetUserHost();

        Token = Session["GitToken"] as IManageUser;
        if (Token == null)
        {
            // 如果未登录，则自动跳转登录
            var provider = ManageProvider.Provider;
            var user = provider.Current ?? provider.TryLogin(HttpContext);
            //if (user == null)
            //{
            //    var url = "/Admin/User/Login";
            //    var returnUrl = Request.GetRawUrl();
            //    if (returnUrl != null) url += "?r=" + HttpUtility.UrlEncode(returnUrl.PathAndQuery);

            //    filterContext.Result = new RedirectResult(url);

            //    return;
            //}

            // 自动创建本地用户
            if (user != null)
            {
                Token = UserX.GetOrAdd(user);
                Session["GitToken"] = Token;
            }
        }

        ViewBag.Token = Token;

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