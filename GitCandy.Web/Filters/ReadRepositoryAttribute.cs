using GitCandy.Controllers;
using System.Web.Mvc;

namespace GitCandy.Filters
{
    public class ReadRepositoryAttribute : SmartAuthorizeAttribute
    {
        private bool requireWrite;

        public ReadRepositoryAttribute(bool requireWrite = false)
        {
            this.requireWrite = requireWrite;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;

            var repoController = controller as RepositoryController;
            if (repoController != null)
            {
                var username = controller.Token == null ? null : controller.Token.Username;
                var owner = controller.ValueProvider.GetValue("owner");
                var field = controller.ValueProvider.GetValue("name");
                var canRead = owner != null && field != null && (requireWrite
                       ? repoController.RepositoryService.CanWriteRepository(owner.AttemptedValue, field.AttemptedValue, username)
                       : repoController.RepositoryService.CanReadRepository(owner.AttemptedValue, field.AttemptedValue, username));
                if (canRead) return;
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}