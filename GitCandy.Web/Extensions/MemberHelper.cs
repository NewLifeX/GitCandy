using NewLife.Model;
using XCode.Membership;

namespace GitCandy.Web.Extensions;

public static class MemberHelper
{
    /// <summary>是否管理员</summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static Boolean IsAdmin(this IManageUser user) => user != null && user is User au && au.Roles.Any(e => e.IsSystem);
}