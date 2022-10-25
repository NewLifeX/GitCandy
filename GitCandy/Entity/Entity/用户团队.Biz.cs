/*
 * XCoder v6.8.6160.27608
 * 作者：Stone/X2
 * 时间：2016-11-21 15:48:51
 * 版权：版权所有 (C) 新生命开发团队 2002~2016
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using XCode;
using XCode.DataAccessLayer;
using XCode.Membership;

namespace NewLife.GitCandy.Entity;

/// <summary>用户团队</summary>
public partial class UserTeam : LogEntity<UserTeam>
{
    #region 对象操作
    static UserTeam()
    {
        Meta.Modules.Add<UserModule>();
        Meta.Modules.Add<TimeModule>();
        Meta.Modules.Add<IPModule>();
    }

    public override void Valid(Boolean isNew)
    {
        if (UserID <= 0) throw new ArgumentNullException(__.UserID, _.UserID.DisplayName);
        if (TeamID <= 0) throw new ArgumentNullException(__.TeamID, _.TeamID.DisplayName);

        base.Valid(isNew);
    }
    #endregion

    #region 扩展属性
    /// <summary>团队</summary>
    [XmlIgnore, IgnoreDataMember]
    public User User => Extends.Get(nameof(User), k => User.FindByID(UserID));

    /// <summary>用户名称</summary>
    [DisplayName("用户")]
    [Map(__.UserID, typeof(User), "ID")]
    public String UserName => User + "";

    /// <summary>团队</summary>
    [XmlIgnore, IgnoreDataMember]
    public User Team => Extends.Get(nameof(Team), k => User.FindByID(TeamID));

    /// <summary>团队名称</summary>
    [DisplayName("团队")]
    [Map(__.TeamID, typeof(User), "ID")]
    public String TeamName => Team + "";
    #endregion

    #region 扩展查询

    /// <summary>根据用户、团队查找</summary>
    /// <param name="userid">用户</param>
    /// <param name="teamid">团队</param>
    /// <returns></returns>
    [DataObjectMethod(DataObjectMethodType.Select, false)]
    public static UserTeam FindByUserIDAndTeamID(Int32 userid, Int32 teamid)
    {
        if (userid <= 0 || teamid <= 0) return null;

        if (Meta.Count >= 1000)
            return Find(new String[] { __.UserID, __.TeamID }, new Object[] { userid, teamid });
        else // 实体缓存
            return Meta.Cache.Entities.FirstOrDefault(e => e.UserID == userid && e.TeamID == teamid);
    }

    public static UserTeam FindByUserAndTeam(String username, String teamname)
    {
        if (username.IsNullOrEmpty() || teamname.IsNullOrEmpty()) return null;

        var team = User.FindByName(teamname);
        if (team == null) return null;

        var user = User.FindByName(username);
        if (user == null) return null;

        return FindByUserIDAndTeamID(user.ID, team.ID);
    }

    public static IList<UserTeam> FindAllByUserID(Int32 userid)
    {
        if (userid <= 0) return new List<UserTeam>();

        if (Meta.Count >= 1000)
            return FindAll(_.UserID == userid);
        else
            return Meta.Cache.Entities.Where(e => e.UserID == userid).ToList();
    }

    public static IList<UserTeam> FindAllByTeamID(Int32 teamid)
    {
        if (teamid <= 0) return new List<UserTeam>();

        if (Meta.Count >= 1000)
            return FindAll(_.TeamID == teamid);
        else
            return Meta.Cache.Entities.Where(e => e.TeamID == teamid).ToList();
    }
    #endregion

    #region 高级查询
    // 以下为自定义高级查询的例子

    /// <summary>查询满足条件的记录集，分页、排序</summary>
    /// <param name="userid">用户编号</param>
    /// <param name="start">开始时间</param>
    /// <param name="end">结束时间</param>
    /// <param name="key">关键字</param>
    /// <param name="param">分页排序参数，同时返回满足条件的总记录数</param>
    /// <returns>实体集</returns>
    public static IList<UserTeam> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
    {
        // WhereExpression重载&和|运算符，作为And和Or的替代
        // SearchWhereByKeys系列方法用于构建针对字符串字段的模糊搜索，第二个参数可指定要搜索的字段
        var exp = SearchWhereByKeys(key, null, null);

        if (userid > 0) exp &= _.UserID == userid;
        exp &= _.UpdateTime.Between(start, end);

        return FindAll(exp, param);
    }

    public static SelectBuilder SearchSql(Int32 teamId) => FindSQL(_.TeamID == teamId, null, _.UserID);
    #endregion

    #region 扩展操作
    #endregion

    #region 业务
    public static UserTeam Add(Int32 userid, Int32 teamid, Boolean isadmin)
    {
        var ut = FindByUserIDAndTeamID(userid, teamid);
        ut ??= new UserTeam();
        ut.UserID = userid;
        ut.TeamID = teamid;
        ut.IsAdmin = isadmin;
        ut.Save();

        return ut;
    }

    public override String ToString() => $"{UserName},{TeamName}";
    #endregion
}