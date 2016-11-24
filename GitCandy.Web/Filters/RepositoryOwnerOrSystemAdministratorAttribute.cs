using GitCandy.Controllers;
using System.Web.Mvc;

namespace GitCandy.Filters
{
    public class RepositoryOwnerOrSystemAdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null)
            {
                if (controller.Token.IsAdmin)
                    return;

                var repoController = controller as RepositoryController;
                if (repoController != null)
                {
                    var owner = controller.ValueProvider.GetValue("owner");
                    var field = controller.ValueProvider.GetValue("name");
                    var isAdmin = field != null && repoController.RepositoryService.IsRepositoryAdministrator(owner.AttemptedValue, field.AttemptedValue, controller.Token.Username);
                    if (isAdmin) return;
                }
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}