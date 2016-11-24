using GitCandy.Controllers;
using System.Web.Mvc;

namespace GitCandy.Filters
{
    /// <summary>团队或系统管理员</summary>
    public class TeamOrSystemAdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null)
            {
                if (controller.Token.IsAdmin) return;

                var field = controller.ValueProvider.GetValue("name");
                var isAdmin = field != null && controller.MembershipService.IsTeamAdministrator(field.AttemptedValue, controller.Token.Username);

                if (isAdmin) return;
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}