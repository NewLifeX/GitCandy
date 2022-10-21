using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.Model;

namespace GitCandy.Web.Filters;

/// <summary>当前用户或系统管理员</summary>
public class CurrentUserOrAdministratorAttribute : SmartAuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        if (filterContext.Result != null) return;

        var user = filterContext.HttpContext.User?.Identity as IManageUser;
        if (user != null)
        {
            if (user.IsAdmin()) return;

            var field = filterContext.RouteData.DataTokens["name"];
            if (field == null) return;

            var name = field.AttemptedValue;
            if (name.IsNullOrEmpty() || user?.Name == name) return;
        }

        HandleUnauthorizedRequest(filterContext);
    }
}