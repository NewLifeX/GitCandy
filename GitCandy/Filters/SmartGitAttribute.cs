using System;
using System.Text;
using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Controllers;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Filters
{
    public class SmartGitAttribute : SmartAuthorizeAttribute
    {
        private const String AuthKey = "GitCandyGitAuthorize";

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.Controller as GitController;
            if (controller == null) return;

            // git.exe not accept cookies as well as no session available
            var username = controller.Session[AuthKey] as String;
            if (username == null)
            {
                var token = controller.Token;
                if (token != null) username = token.Username;
            }
            if (username == null)
            {
                var auth = controller.HttpContext.Request.Headers["Authorization"];

                if (!String.IsNullOrEmpty(auth))
                {
                    var bytes = Convert.FromBase64String(auth.Substring(6));
                    var certificate = Encoding.ASCII.GetString(bytes);
                    var index = certificate.IndexOf(':');
                    var password = certificate.Substring(index + 1);
                    username = certificate.Substring(0, index);

                    var user = UserX.Check(username, password);
                    username = user != null ? user.Name : null;
                }
            }

            controller.Session[AuthKey] = username;

            if (username == null && !UserConfiguration.Current.IsPublicServer)
            {
                HandleUnauthorizedRequest(filterContext);
                return;
            }

            var right = false;

            var owner = controller.ValueProvider.GetValue("owner")?.AttemptedValue;
            var project = controller.ValueProvider.GetValue("project")?.AttemptedValue;
            var service = controller.ValueProvider.GetValue("service")?.AttemptedValue;

            if (String.IsNullOrEmpty(service)) // redirect to git browser
            {
                right = true;
            }
            else if (String.Equals(service, "git-receive-pack", StringComparison.OrdinalIgnoreCase)) // git push
            {
                right = controller.RepositoryService.CanWriteRepository(owner, project, username);
            }
            else if (String.Equals(service, "git-upload-pack", StringComparison.OrdinalIgnoreCase)) // git fetch
            {
                right = controller.RepositoryService.CanReadRepository(owner, project, username);
            }

            if (!right)
                HandleUnauthorizedRequest(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var controller = filterContext.Controller as CandyControllerBase;
            if (controller == null || controller.Token == null)
            {
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"GitCandy\"");
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}