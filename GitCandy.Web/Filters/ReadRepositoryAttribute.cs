using System.Web.Mvc;
using GitCandy.Controllers;

namespace GitCandy.Filters
{
    /// <summary>读写代码库</summary>
    public class ReadRepositoryAttribute : SmartAuthorizeAttribute
    {
        private System.Boolean requireWrite;

        public ReadRepositoryAttribute(System.Boolean requireWrite = false)
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
                var username = controller.Token?.Username;
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