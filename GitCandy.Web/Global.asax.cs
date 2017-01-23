using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GitCandy.Base;
using GitCandy.Git.Cache;
using NewLife.Log;

namespace GitCandy
{
    public class GitCandyApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            XTrace.WriteVersion(this.GetType().Assembly);

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GitCacheAccessor.Initialize();
            SshServerConfig.StartSshServer();
        }

        protected void Application_AcquireRequestState()
        {
            if (HttpContext.Current.Session != null)
            {
                var culture = Session["Culture"] as CultureInfo;
                if (culture == null)
                {
                    String langName = "en-us";

                    if (Request.Cookies["Lang"] != null)
                    {
                        langName = Request.Cookies["Lang"].Value;
                    }
                    else if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length != 0)
                    {
                        langName = HttpContext.Current.Request.UserLanguages[0].Split(';')[0];
                    }
                    try
                    {
                        culture = CultureInfo.CreateSpecificCulture(langName);
                    }
                    catch
                    {
                        culture = CultureInfo.CreateSpecificCulture("en-us");
                    }

                    var cookie = new HttpCookie("Lang", culture.Name)
                    {
                        Expires = DateTime.Now.AddYears(2),
                    };
                    Response.Cookies.Set(cookie);

                    Session["Culture"] = culture;
                }
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        protected void Application_BeginRequest()
        {
            Profiler.Start();
        }
    }
}