using NewLife.Model;
using XCode.Membership;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Extensions;

public static class MemberHelper
{
    /// <summary>是否管理员</summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static Boolean IsAdmin(this IManageUser user)
    {
        if (user == null) return false;

        if (user is User au) return au.Roles.Any(e => e.IsSystem);
        if (user is UserX ux) return ux.IsAdmin;

        return false;
    }
}