using NewLife;

namespace GitCandy.Web.Base;

class GitUrlConstraint : IRouteConstraint
{
    private static HashSet<String> _cache = new HashSet<String>(StringComparer.OrdinalIgnoreCase)
    {
        "info/refs","git-upload-pack","git-receive-pack"
    };

    public Boolean Match(HttpContext httpContext, IRouter route, String routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        var name = values[routeKey] + "";
        if (name.IsNullOrEmpty()) return false;

        return _cache.Contains(name);
    }
}
