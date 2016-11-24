using System;
using System.Web.Mvc;
using GitCandy.Controllers;

namespace GitCandy.Filters
{
    /// <summary>当前用户或系统管理员</summary>
    public class CurrentUserOrAdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as CandyControllerBase;
            if (controller != null && controller.Token != null)
            {
                if (controller.Token.IsAdmin) return;

                var field = filterContext.Controller.ValueProvider.GetValue("name");
                if (field == null) return;

                var name = field.AttemptedValue;
                if (name.IsNullOrEmpty() || controller.Token.Username == name) return;
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}