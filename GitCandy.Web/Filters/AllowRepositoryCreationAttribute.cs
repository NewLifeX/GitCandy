using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Controllers;
using GitCandy.Extensions;

namespace GitCandy.Filters
{
    /// <summary>允许注册</summary>
    public class AllowRepositoryCreationAttribute : SmartAuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var token = (filterContext.Controller as CandyControllerBase)?.Token;
            if (token != null && (UserConfiguration.Current.AllowRepositoryCreation || token.IsAdmin())) return;

            HandleUnauthorizedRequest(filterContext);
        }
    }
}