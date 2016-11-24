using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Controllers;

namespace GitCandy.Filters
{
    /// <summary>允许注册</summary>
    public class AllowRepositoryCreationAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null
                && (UserConfiguration.Current.AllowRepositoryCreation
                    || controller.Token.IsAdmin))
                return;

            HandleUnauthorizedRequest(filterContext);
        }
    }
}