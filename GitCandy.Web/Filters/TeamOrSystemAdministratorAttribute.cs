using GitCandy.Web.Controllers;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife.GitCandy.Entity;

namespace GitCandy.Web.Filters;

/// <summary>团队或系统管理员</summary>
public class TeamOrSystemAdministratorAttribute : SmartAuthorizeAttribute
{
    public Boolean RequireAdmin { get; set; }

    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        var controller = filterContext.Controller as CandyControllerBase;
        if (controller != null && controller.Token != null)
        {
            if (controller.Token.IsAdmin()) return;

            var field = controller.ValueProvider.GetValue("name");
            //var isAdmin = field != null && controller.MembershipService.InTeam(field.AttemptedValue, controller.Token?.Name);

            //if (isAdmin) return;
            if (field != null)
            {
                var role = UserTeam.FindByUserAndTeam(controller.Token?.Name, field.AttemptedValue);
                if (role != null)
                    // 不要求管理员，或者本身就是管理员
                    if (!RequireAdmin || role.IsAdmin) return;
            }
        }

        HandleUnauthorizedRequest(filterContext);
    }
}