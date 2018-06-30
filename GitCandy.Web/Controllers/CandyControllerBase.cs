using System;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Data;
using NewLife.Log;
using NewLife.Model;
using NewLife.Reflection;
using XCode.Membership;
using UserY = NewLife.GitCandy.Entity.User;

namespace GitCandy.Controllers
{
    [AllowAnonymous]
    public abstract class CandyControllerBase : Controller
    {
        //private const String AuthKey = "_gc_auth";

        //private Token _token;

        public MembershipService MembershipService { get; set; } = new MembershipService();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var prv = ManageProvider.Provider;
            if (prv.TryLogin() != null)
            {
                prv.SetPrincipal();

                // 同步用户数据到Git用户表
                if (UserY.Current == null) UserY.Current = UserY.GetOrAdd(prv.Current);
            }

            base.OnAuthorization(filterContext);
        }

        /// <summary>当前用户</summary>
        public IManageUser Token => User?.Identity as IManageUser;

        static AssemblyX asm = AssemblyX.Create(Assembly.GetExecutingAssembly());
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UserConfiguration.Current.ForceSsl && !Request.IsSecureConnection)
            {
                var uri = Request.Url;
                filterContext.Result = Redirect(new Uri(String.Format("https://{0}:{1}{2}", uri.Host, UserConfiguration.Current.SslPort, uri.PathAndQuery)).ToString());
                XTrace.WriteLine("Redirect to SSL from " + uri);
                return;
            }

            var culture = Thread.CurrentThread.CurrentUICulture;
            var displayName = culture.Name.StartsWith("en")
                ? culture.NativeName
                : culture.EnglishName + " - " + culture.NativeName;

            ViewBag.Language = displayName;
            ViewBag.Lang = culture.Name;
            ViewBag.Identity = 0;

            Response.AddHeader("X-GitCandy-Version", asm.Version);

            base.OnActionExecuting(filterContext);
        }

        protected virtual ActionResult RedirectToStartPage(String returnUrl = null)
        {
            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) return RedirectToAction("Index", "Repository");

            return Redirect(returnUrl);
        }

        //private void CacheToken(Token token)
        //{
        //    if (token != null)
        //        HttpRuntime.Cache.Insert(token.AuthCode.ToString(), token, null, token.Expires, Cache.NoSlidingExpiration);
        //}

        //private Token GetCachedToken(String key)
        //{
        //    return key == null
        //        ? null
        //        : HttpRuntime.Cache.Get(key) as Token;
        //}

        //private void RemoveCachedToken(String key)
        //{
        //    if (key != null)
        //        HttpRuntime.Cache.Remove(key);
        //}
    }
}