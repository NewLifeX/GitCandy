using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Controllers;

namespace GitCandy.Filters
{
    /// <summary>开放服务</summary>
    public class PublicServerAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (UserConfiguration.Current.IsPublicServer) return;

            var des = filterContext.ActionDescriptor;
            var skipAuthorization =
                   des.IsDefined(typeof(AllowAnonymousAttribute), true)
                || des.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || des.IsDefined(typeof(SmartGitAttribute), true)
                || des.ControllerDescriptor.IsDefined(typeof(SmartGitAttribute), true);

            if (skipAuthorization) return;

            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null) return;

            HandleUnauthorizedRequest(filterContext);
        }
    }
}