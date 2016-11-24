using GitCandy.Controllers;
using System;
using System.Web;
using System.Web.Mvc;

namespace GitCandy.Filters
{
    /// <summary>智能验证，主要处理验证失败时的系统逻辑</summary>
    public class SmartAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var controller = filterContext.Controller as CandyControllerBase;
            if (controller == null || controller.Token == null)
            {
                var helper = new UrlHelper(filterContext.RequestContext);

                var retUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
                var retObj = (String.IsNullOrEmpty(retUrl) || retUrl == "/")
                    ? null
                    : new { ReturnUrl = filterContext.HttpContext.Request.Url.PathAndQuery };

                filterContext.Result = new RedirectResult(helper.Action("Login", "Account", retObj));
            }
            else if (controller.Token.IsAdmin)
            {
                throw new HttpException(404, "Project not found.");
            }
            else
            {
                //throw new UnauthorizedAccessException();
                filterContext.Result = new ContentResult { Content = "无权操作！" };
            }
        }
    }
}