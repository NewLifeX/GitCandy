using System.Globalization;
using GitCandy.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewLife.GitCandy.Entity;

namespace GitCandy.Web.Extensions;

public static class HtmlHelperExtension
{
    public static RouteValueDictionary OverRoute(this IHtmlHelper helper, Object routeValues = null, Boolean withQuery = false)
    {
        var old = helper.ViewContext.RouteData.Values;
        if (routeValues == null) return old;

        var over = new Dictionary<String, Object>(old, StringComparer.OrdinalIgnoreCase);
        if (withQuery)
        {
            var qs = helper.ViewContext.HttpContext.Request.Query;
            foreach (var item in qs)
            {
                over[item.Key] = item.Value;
            }
        }
        var values = new RouteValueDictionary(routeValues);
        foreach (var pair in values)
        {
            over[pair.Key] = pair.Value;
        }

        return new RouteValueDictionary(over);
    }

    //public static HtmlString ActionLink(this IHtmlHelper htmlHelper, String linkText, String actionName, RouteValueDictionary routeValues, Object htmlAttributes)
    //{
    //    return htmlHelper.ActionLink(linkText, actionName, routeValues, htmlAttributes.CastToDictionary());
    //}

    //public static HtmlString ActionLink(this IHtmlHelper htmlHelper, String linkText, String actionName, String controllerName, RouteValueDictionary routeValues, Object htmlAttributes)
    //{
    //    return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes.CastToDictionary());
    //}

    public static IHtmlContent CultureActionLink(this IHtmlHelper htmlHelper, String langName)
    {
        var culture = CultureInfo.CreateSpecificCulture(langName);
        var displayName = culture.Name.StartsWith("en")
            ? culture.NativeName
            : culture.EnglishName + " - " + culture.NativeName;

        return htmlHelper.ActionLink(displayName, "Language", "Home", new { Lang = culture.Name }, null);
    }

    //public static dynamic GetRootViewBag(this IHtmlHelper html)
    //{
    //    var controller = html.ViewContext.Controller;
    //    while (controller.ControllerContext.IsChildAction)
    //    {
    //        controller = controller.ControllerContext.ParentActionViewContext.Controller;
    //    }

    //    return controller.ViewBag;
    //}

    public static HtmlString Link(this IHtmlHelper html, RepositoryModelBase repo)
    {
        if (repo == null) return null;

        var user = User.FindByName(repo.Owner);
        if (user == null) return null;

        var link1 = html.ActionLink(repo.Owner, "Detail", user.IsTeam ? "Team" : "Account", new { name = repo.Owner }, null);
        //var link2 = html.ActionLink(repo.Name, "Tree", new { owner = repo.Owner, name = repo.Name, path = "" });
        var link2 = html.RouteLink(repo.Name, "UserGitWeb", new { owner = repo.Owner, name = repo.Name });

        return new HtmlString(link1.ToString() + "/" + link2.ToString());
    }
}