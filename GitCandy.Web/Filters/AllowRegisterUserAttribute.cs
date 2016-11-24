using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Controllers;

namespace GitCandy.Filters
{
    /// <summary>允许注册</summary>
    public class AllowRegisterUserAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            var currentUser = controller == null ? null : controller.Token;
            if (currentUser != null && currentUser.IsAdmin)
                return;

            if (currentUser == null && UserConfiguration.Current.AllowRegisterUser)
                return;

            HandleUnauthorizedRequest(filterContext);
        }
    }
}