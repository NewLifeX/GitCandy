using GitCandy.Web.Extensions;
using GitCandy.Web.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.Model;

namespace GitCandy.Filters;

/// <summary>系统管理员</summary>
public class AdministratorAttribute : SmartAuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        if (filterContext.Result != null) return;

        var user = filterContext.HttpContext.User?.Identity as IManageUser;
        if (user == null || !user.IsAdmin())
            HandleUnauthorizedRequest(filterContext);
    }
}