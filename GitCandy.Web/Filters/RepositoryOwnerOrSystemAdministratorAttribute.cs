using System.Web.Mvc;
using GitCandy.Controllers;
using GitCandy.Extensions;

namespace GitCandy.Filters
{
    /// <summary>拥有代码库</summary>
    public class RepositoryOwnerOrSystemAdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null)
            {
                if (controller.Token.IsAdmin()) return;

                var repoController = controller as RepositoryController;
                if (repoController != null)
                {
                    var owner = controller.ValueProvider.GetValue("owner");
                    var field = controller.ValueProvider.GetValue("name");
                    var isAdmin = field != null && repoController.RepositoryService.IsRepositoryAdministrator(owner.AttemptedValue, field.AttemptedValue, controller.Token?.Name);
                    if (isAdmin) return;
                }
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}