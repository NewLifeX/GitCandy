using GitCandy.Configuration;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.Model;

namespace GitCandy.Web.Filters;

/// <summary>允许注册</summary>
public class AllowRepositoryCreationAttribute : SmartAuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        if (filterContext.Result != null) return;

        var user = filterContext.HttpContext.User?.Identity as IManageUser;

        if (user != null && (UserConfiguration.Current.AllowRepositoryCreation || user.IsAdmin())) return;

        HandleUnauthorizedRequest(filterContext);
    }
}