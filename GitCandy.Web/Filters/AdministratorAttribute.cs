using System.Web.Mvc;
using GitCandy.Controllers;
using GitCandy.Extensions;

namespace GitCandy.Filters
{
    /// <summary>系统管理员</summary>
    public class AdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller == null || controller.Token == null || !controller.Token.IsAdmin())
                HandleUnauthorizedRequest(filterContext);
        }
    }
}