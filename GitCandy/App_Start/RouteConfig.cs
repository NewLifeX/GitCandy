using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GitCandy.Controllers;
using NewLife.GitCandy.Entity;

namespace GitCandy
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region GitController
            routes.MapRoute(
                name: "UserGitWeb",
                url: "{user}/{name}/{*path}",
                defaults: new { controller = "Repository", action = "Tree" },
                constraints: new { user = new UserUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "UserGit",
                url: "{user}/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                constraints: new { user = new UserUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "TeamGitWeb",
                url: "{team}/{name}/{*path}",
                defaults: new { controller = "Repository", action = "Tree" },
                constraints: new { team = new TeamUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "TeamGit",
                url: "{team}/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                constraints: new { team = new TeamUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region AccountContorller
            // 实现用户名直达用户首页
            routes.MapRoute(
                name: "UserIndex",
                url: "{name}",
                defaults: new { controller = "Account", action = "Detail" },
                constraints: new { name = new UserUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{name}",
                defaults: new { controller = "Account" },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region TeamContorller
            // 实现团队名直达团队首页
            routes.MapRoute(
                name: "TeamIndex",
                url: "{name}",
                defaults: new { controller = "Team", action = "Detail" },
                constraints: new { name = new TeamUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
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
                name: "Home",
                url: "Home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { typeof(Controllers.HomeController).Namespace }
            );
            routes.MapRoute(
                name: "HomeIndex",
                url: "",
                defaults: new { controller = "Repository", action = "Index" },
                namespaces: new[] { typeof(Controllers.HomeController).Namespace }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { typeof(Controllers.HomeController).Namespace }
            );
        }
    }

    class UserUrlConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var name = values[parameterName] + "";
            if (name.IsNullOrEmpty()) return false;

            if (User.FindByName(name) != null) return true;

            return false;
        }
    }

    class TeamUrlConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var name = values[parameterName] + "";
            if (name.IsNullOrEmpty()) return false;

            if (Team.FindByName(name) != null) return true;

            return false;
        }
    }
}