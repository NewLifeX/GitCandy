using System;
using NewLife.Model;
using XCode.Membership;

namespace GitCandy.Extensions
{
    public static class MemberHelper
    {
        /// <summary>是否管理员</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Boolean IsAdmin(this IManageUser user) => user != null && user is IUser au && au.RoleName == "管理员";
    }
}