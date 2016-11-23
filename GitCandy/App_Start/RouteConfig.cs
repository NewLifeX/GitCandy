﻿using System;
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
                url: "{user}/{name}/{*path}",
                defaults: new { controller = "Repository", action = "Tree" },
                constraints: new { user = new UserUrlConstraint() },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            routes.MapRoute(
                name: "UserGit",
                url: "{user}/{project}/{*verb}",
                defaults: new { controller = "Git", action = "Smart" },
                constraints: new { user = new UserUrlConstraint() },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            #endregion

            #region AccountContorller
            // 实现用户名直达用户首页
            routes.MapRoute(
                name: "UserIndex",
                url: "{name}",
                defaults: new { controller = "User", action = "Detail" },
                constraints: new { name = new UserUrlConstraint() },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            routes.MapRoute(
                name: "User",
                url: "User/{action}/{name}",
                defaults: new { controller = "User" },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            #endregion

            #region TeamContorller
            routes.MapRoute(
                name: "Team",
                url: "Team/{action}/{name}",
                defaults: new { controller = "Team" },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            #endregion

            #region RepositoryController
            routes.MapRoute(
                name: "Repository",
                url: "Repository/{action}/{name}/{*path}",
                defaults: new { controller = "Repository", path = "" },
                namespaces: new[] { typeof(UserController).Namespace }
            );
            #endregion

            #region SettingController
            routes.MapRoute(
                name: "Setting",
                url: "Setting/{action}",
                defaults: new { controller = "Setting", action = "Edit" },
                namespaces: new[] { typeof(UserController).Namespace }
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
        public bool Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var name = values[parameterName] + "";
            if (name.IsNullOrEmpty()) return false;

            return Match(name);
            //if (User.FindByName(name) != null) return true;

            //return false;
        }

        private static DictionaryCache<String, Boolean> _cache;
        private static Boolean Match(String name)
        {
            if (_cache == null)
            {
                _cache = new DictionaryCache<String, Boolean>(StringComparer.OrdinalIgnoreCase);
                _cache.Asynchronous = true;
                _cache.CacheDefault = true;
                _cache.Expire = 10 * 60;        // 10分钟过期
                //_cache.ClearPeriod = 10 * 60;   // 10分钟清理一次过期项
            }

            return _cache.GetItem(name, k => User.FindByName(k) != null);
        }
    }
}