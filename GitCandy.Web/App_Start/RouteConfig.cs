using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GitCandy.Controllers;
using NewLife.Collections;
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
                url: "{owner}/{name}",
                defaults: new { controller = "Repository", action = "Tree" },
                constraints: new { owner = new UserUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            //routes.MapRoute(
            //    name: "UserGitWeb2",
            //    url: "{owner}/{name}/{*path}",
            //    defaults: new { controller = "Repository", action = "Tree", path = UrlParameter.Optional },
            //    constraints: new { owner = new UserUrlConstraint() },
            //    namespaces: new[] { typeof(AccountController).Namespace }
            //);
            routes.MapRoute(
                name: "UserGit",
                url: "{owner}/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                constraints: new { owner = new UserUrlConstraint(), verb = new GitUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "UserGitAct",
                url: "{owner}/{name}/{action}/{*path}",
                defaults: new { controller = "Repository", path = UrlParameter.Optional },
                constraints: new { owner = new UserUrlConstraint() },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            #endregion

            #region AccountContorller
            // 实现用户名直达用户首页
            routes.MapRoute(
                name: "UserIndex",
                url: "{name}",
                defaults: new { controller = "Account", action = "Detail" },
                constraints: new { name = new UserUrlConstraint { IsTeam = false } },
                namespaces: new[] { typeof(AccountController).Namespace }
            );
            routes.MapRoute(
                name: "User",
                url: "User/{action}/{name}",
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
                constraints: new { name = new UserUrlConstraint { IsTeam = true } },
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
        public Boolean? IsTeam { get; set; }

        public bool Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var name = values[parameterName] + "";
            if (name.IsNullOrEmpty()) return false;

            var m = Match(name);

            if (IsTeam == null) return m != null;

            return IsTeam == m;
        }

        private static DictionaryCache<String, Boolean?> _cache;
        private static Boolean? Match(String name)
        {
            if (_cache == null)
            {
                _cache = new DictionaryCache<String, Boolean?>(StringComparer.OrdinalIgnoreCase);
                _cache.Asynchronous = true;
                _cache.CacheDefault = true;
                _cache.Expire = 10 * 60;        // 10分钟过期
                //_cache.ClearPeriod = 10 * 60;   // 10分钟清理一次过期项
            }

            return _cache.GetItem(name, k => User.FindByName(k)?.IsTeam);
        }
    }

    class GitUrlConstraint : IRouteConstraint
    {
        private static HashSet<String> _cache = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "info/refs","git-upload-pack","git-receive-pack"
        };

        public bool Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var name = values[parameterName] + "";
            if (name.IsNullOrEmpty()) return false;

            return _cache.Contains(name);
        }
    }

    //class TeamUrlConstraint : IRouteConstraint
    //{
    //    public bool Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    //    {
    //        var name = values[parameterName] + "";
    //        if (name.IsNullOrEmpty()) return false;

    //        return Match(name);
    //    }

    //    private static DictionaryCache<String, Boolean> _cache;
    //    private static Boolean Match(String name)
    //    {
    //        if (_cache == null)
    //        {
    //            _cache = new DictionaryCache<String, Boolean>(StringComparer.OrdinalIgnoreCase);
    //            _cache.Asynchronous = true;
    //            _cache.CacheDefault = true;
    //            _cache.Expire = 10 * 60;        // 10分钟过期
    //            //_cache.ClearPeriod = 10 * 60;   // 10分钟清理一次过期项
    //        }

    //        return _cache.GetItem(name, k =>
    //        {
    //            var user = User.FindByName(k);
    //            if (user == null) return false;
    //            return user.IsTeam;
    //        });
    //    }
    //}
}