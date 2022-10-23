using NewLife;
using NewLife.Caching;
using NewLife.GitCandy.Entity;

namespace GitCandy.Web.Base;

class UserUrlConstraint : IRouteConstraint
{
    public Boolean? IsTeam { get; set; }

    public Boolean Match(HttpContext httpContext, IRouter route, String routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        var name = values[routeKey] + "";
        if (name.IsNullOrEmpty()) return false;

        var m = Match(name);

        if (IsTeam == null) return m != null;

        return IsTeam == m;
    }

    private static ICache _cache = new MemoryCache();
    private static Boolean? Match(String name)
    {
        var user = _cache.Get<User>(name);
        if (user != null) return user.IsTeam;

        user = User.FindByName(name);

        _cache.Set(name, user, 10 * 60);

        return user?.IsTeam;
    }
}