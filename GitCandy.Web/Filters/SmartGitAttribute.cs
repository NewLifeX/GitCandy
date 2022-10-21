using System.Text;
using GitCandy.Configuration;
using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using XCode.Membership;

namespace GitCandy.Web.Filters;

/// <summary>Git客户端访问验证</summary>
public class SmartGitAttribute : SmartAuthorizeAttribute
{
    private const String AuthKey = "GitCandyGitAuthorize";

    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        var controller = filterContext.Controller as GitController;
        if (controller == null) return;

        // git.exe not accept cookies as well as no session available
        var username = controller.Session[AuthKey] as String;
        if (username == null)
        {
            var token = controller.Token;
            if (token != null) username = token.Name;
        }
        if (username == null)
        {
            // 从Http基本验证获取信息进行登录
            var auth = controller.HttpContext.Request.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                var bytes = Convert.FromBase64String(auth.Substring(6));
                var certificate = Encoding.ASCII.GetString(bytes);
                var index = certificate.IndexOf(':');
                var password = certificate[(index + 1)..];
                username = certificate[..index];

                // 登录验证
                //var user = UserX.Check(username, password);
                //username = user != null && user.Login(password) ? user.Name : null;
                var user = ManageProvider.Provider.Login(username, password);
                username = user?.Name;
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
            right = true;
        else if (String.Equals(service, "git-receive-pack", StringComparison.OrdinalIgnoreCase)) // git push
            right = controller.RepositoryService.CanWriteRepository(owner, project, username);
        else if (String.Equals(service, "git-upload-pack", StringComparison.OrdinalIgnoreCase)) // git fetch
            right = controller.RepositoryService.CanReadRepository(owner, project, username);

        if (!right)
            HandleUnauthorizedRequest(filterContext);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        var controller = filterContext.Controller as CandyControllerBase;
        if (controller == null || controller.Token == null)
        {
            // 要求客户端提供基本验证的用户名和密码
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"GitCandy\"");

            // 基本验证是明文传输密码，本想改为摘要验证，但是那样传输过来的是密码混合其它新的的签名，无法跟数据库对比
            //var sb = new StringBuilder();
            //sb.Append("Digest");
            //sb.Append(" realm=\"GitCandy\"");
            //sb.Append(",qop=\"auth,auth-int\"");
            //sb.Append(",nonce=\"66C4EF58DA7CB956BD04233FBB64E0A4\"");
            //filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", sb.ToString());

            filterContext.Result = new HttpUnauthorizedResult();
        }
        else
            throw new UnauthorizedAccessException();
    }
}