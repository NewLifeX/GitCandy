using System;
using System.Web.Mvc;
using GitCandy.Controllers;
using GitCandy.Extensions;

namespace GitCandy.Filters
{
    /// <summary>当前用户或系统管理员</summary>
    public class CurrentUserOrAdministratorAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var token = (filterContext.Controller as CandyControllerBase)?.Token;
            if (token != null)
            {
                if (token.IsAdmin()) return;

                var field = filterContext.Controller.ValueProvider.GetValue("name");
                if (field == null) return;

                var name = field.AttemptedValue;
                if (name.IsNullOrEmpty() || token?.Name == name) return;
            }

            HandleUnauthorizedRequest(filterContext);
        }
    }
}