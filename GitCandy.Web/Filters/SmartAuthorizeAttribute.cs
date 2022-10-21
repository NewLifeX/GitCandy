using System.Reflection;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using NewLife.Cube;
using NewLife.Model;
using NewLife.Web;
using XCode.Membership;

namespace GitCandy.Web.Filters;

/// <summary>智能验证，主要处理验证失败时的系统逻辑</summary>
public abstract class SmartAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        // 如果已经处理过，就不处理了
        if (filterContext.Result != null) return;

        var ctrl = (ControllerActionDescriptor)filterContext.ActionDescriptor;

        // 允许匿名访问时，直接跳过检查
        if (ctrl.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute)) ||
            ctrl.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute))) return;

        var prv = ManageProvider.Provider;

        // 判断当前登录用户
        var user = ManagerProviderHelper.TryLogin(prv, filterContext.HttpContext);
        if (user == null)
        {
            HandleUnauthorizedRequest(filterContext);
        }
    }

    protected void HandleUnauthorizedRequest(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        var retUrl = httpContext.Request.GetRawUrl().PathAndQuery;

        LogProvider.Provider.WriteLog("访问", "拒绝", false, retUrl, ip: httpContext.GetUserHost());

        if (httpContext.User?.Identity is not IManageUser user)
        {
            var helper = new UrlHelper(context);

            var retObj = String.IsNullOrEmpty(retUrl) || retUrl == "/"
                ? null
                : new { ReturnUrl = retUrl };

            context.Result = new RedirectResult(helper.Action("Login", "Account", retObj));
        }
        else if (user.IsAdmin())
            context.Result = new NotFoundResult();
        else
            //throw new UnauthorizedAccessException();
            context.Result = new ContentResult { Content = "无权操作！" };
    }
}