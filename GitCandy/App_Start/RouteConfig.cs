using System.Web.Mvc;
using System.Web.Routing;
using GitCandy.Controllers;

namespace GitCandy
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region GitController
            routes.MapRoute(
                name: "Git.git",
                url: "git/{project}.git/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "Git",
                url: "git/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region AccountContorller
            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{name}",
                defaults: new { controller = "Account" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region TeamContorller
            routes.MapRoute(
                name: "Team",
                url: "Team/{action}/{name}",
                defaults: new { controller = "Team" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region RepositoryController
            routes.MapRoute(
                name: "Repository",
                url: "Repository/{action}/{name}/{*path}",
                defaults: new { controller = "Repository", path = "" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region SettingController
            routes.MapRoute(
                name: "Setting",
                url: "Setting/{action}",
                defaults: new { controller = "Setting", action = "Edit" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { typeof(Controllers.HomeController).Namespace }
            );
        }
    }
}