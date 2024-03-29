﻿/*
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
using NewLife.Model;
using XCode;
using XCode.Membership;

namespace NewLife.GitCandy.Entity;

/// <summary>用户</summary>
public partial class User : LogEntity<User>, IManageUser/*, IIdentity*/
{
    #region 对象操作
    /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void InitData()
    {
        Meta.Modules.Add<UserModule>();
        Meta.Modules.Add<TimeModule>();
        Meta.Modules.Add<IPModule>();

        var sc = Meta.SingleCache;
        sc.FindSlaveKeyMethod = k => Find(_.Name == k);
        sc.GetSlaveKeyMethod = e => e.Name;
    }

    protected override Int32 OnDelete()
    {
        //Teams?.Delete(true);
        UserTeam.FindAllByUserID(ID).Delete(true);
        UserTeam.FindAllByTeamID(ID).Delete(true);
        //Repositories?.Delete(true);
        UserRepository.FindAllByUserID(ID).Delete();
        Repository.FindAllByOwnerID(ID).Delete();
        //AuthorizationLog.FindAllByUserID(ID).Delete();

        return base.OnDelete();
    }
    #endregion

    #region 扩展属性
    /// <summary>团队关系</summary>
    [XmlIgnore, IgnoreDataMember]
    public IList<UserTeam> Teams => Extends.Get(nameof(Teams), k => UserTeam.FindAllByUserID(ID));

    [XmlIgnore, IgnoreDataMember]
    public String[] TeamNames => Teams?.Select(e => e.Team?.Name).OrderBy(e => e).ToArray();

    /// <summary>仓库关系</summary>
    [XmlIgnore, IgnoreDataMember]
    public IList<UserRepository> Repositories => Extends.Get(nameof(Repositories), k => UserRepository.FindAllByUserID(ID));

    [XmlIgnore, IgnoreDataMember]
    public String[] RepositoryNames => Repositories?.Select(e => e.RepositoryName).ToArray();
    #endregion

    #region 扩展查询
    public static User FindByID(Int32 id)
    {
        if (id <= 0) return null;

        if (Meta.Count < 1000) return Meta.Cache.Entities.FirstOrDefault(e => e.ID == id);

        return Meta.SingleCache[id];
    }

    /// <summary>根据名称。登录用户名查找</summary>
    /// <param name="name">名称。登录用户名</param>
    /// <returns></returns>
    [DataObjectMethod(DataObjectMethodType.Select, false)]
    public static User FindByName(String name)
    {
        if (name.IsNullOrEmpty()) return null;

        if (Meta.Count < 1000) return Meta.Cache.Entities.FirstOrDefault(e => e.Name.EqualIgnoreCase(name));

        // 单对象缓存
        return Meta.SingleCache.GetItemWithSlaveKey(name) as User;
    }

    public static User FindByEmail(String email)
    {
        if (email.IsNullOrEmpty() || !email.Contains("@") || !email.Contains(".")) return null;

        if (Meta.Count >= 1000)
            return Find(__.Email, email);
        else // 实体缓存
            return Meta.Cache.Entities.FirstOrDefault(e => e.Email.EqualIgnoreCase(email));
        // 单对象缓存
        //return Meta.SingleCache[name];
    }
    #endregion

    #region 高级查询
    public static IList<User> Search(Int32 ownerId, Boolean? enable, Boolean? isTeam, DateTime start, DateTime end, String key, PageParameter param)
    {
        // WhereExpression重载&和|运算符，作为And和Or的替代
        // SearchWhereByKeys系列方法用于构建针对字符串字段的模糊搜索，第二个参数可指定要搜索的字段
        var exp = SearchWhereByKeys(key, null, null);

        if (ownerId > 0) exp &= _.ID.In(UserTeam.SearchSql(ownerId));
        if (enable != null) exp &= _.Enable == enable;
        if (isTeam != null) exp &= _.IsTeam == isTeam;

        exp &= _.LastLogin.Between(start, end);

        return FindAll(exp, param);
    }

    public static IList<User> SearchByName(String name, PageParameter param)
    {
        var exp = new WhereExpression();
        exp &= _.IsTeam == false;
        if (!name.IsNullOrEmpty()) exp &= _.Name.Contains(name);

        return FindAll(exp, param);
    }

    public static IList<User> SearchTeam(String name, PageParameter param)
    {
        var exp = new WhereExpression();
        exp &= _.IsTeam == true;
        if (!name.IsNullOrEmpty()) exp &= _.Name.Contains(name);

        return FindAll(exp, param);
    }

    /// <summary>搜索用户，不包含团队</summary>
    /// <param name="key"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IList<User> SearchUser(String key, PageParameter param)
    {
        var exp = new WhereExpression();
        exp &= _.IsTeam == false;
        if (!key.IsNullOrEmpty()) exp &= _.Name.Contains(key) | _.Email.Contains(key);

        return FindAll(exp, param);
    }
    #endregion

    #region 扩展操作
    public override String ToString() => NickName ?? Name;
    #endregion

    #region 业务
    public static User Create(String name, String nickname, String password, String email, String description)
    {
        var user = User.FindByName(name);
        if (user != null) throw new ArgumentException(_.Name.DisplayName + "已存在", __.Name);

        user = User.FindByEmail(email);
        if (user != null) throw new ArgumentException(_.Email.DisplayName + "已存在", __.Email);

        user = new User
        {
            Name = name,
            NickName = nickname,
            Email = email,
            Enable = true,
        };

        user.Save();

        return user;
    }

    public static User CreateTeam(String name, String nickname, String description)
    {
        var user = FindByName(name);
        if (user != null) throw new ArgumentException(_.Name.DisplayName + "已存在", __.Name);

        user = new User
        {
            Name = name,
            NickName = nickname,
            Enable = false,
            IsTeam = true,
        };

        user.Save();

        return user;
    }

    //public Boolean Login(String password)
    //{
    //    var user = this;
    //    if (!user.Enable) return false;

    //    // 清空密码后，任意密码可以登录，并成为新密码
    //    if (user.Password.IsNullOrEmpty())
    //        user.Password = password.MD5();
    //    else
    //    {
    //        if (user.Password != password.MD5()) return false;
    //    }

    //    user.Logins++;
    //    user.LastLogin = DateTime.Now;
    //    user.LastLoginIP = WebHelper.UserHost;
    //    user.Save();

    //    return true;
    //}

    //public static User Check(String name, String pass)
    //{
    //    var user = FindByName(name) ?? FindByEmail(name);
    //    if (user == null) return null;

    //    if (!user.Enable) return null;
    //    if (user.Password != pass.MD5()) return null;

    //    return user;
    //}

    public static User GetOrAdd(Int32 linkid, String name, String mail)
    {
        var user = Find(_.LinkID == linkid);
        user ??= FindByName(name);
        user ??= FindByEmail(mail);
        if (user != null)
        {
            if (user.LinkID > 0 && user.LinkID != linkid) throw new InvalidOperationException($"账号[{name}]被[{user.LinkID}]和[{linkid}]共用，请联系管理员");

            user.LinkID = linkid;
            user.SaveAsync();
        }
        else
        {
            user = new User { LinkID = linkid, Name = name, Enable = true };
            user.Insert();
        }

        //user.Logins++;
        //user.LastLogin = DateTime.Now;
        //user.LastLoginIP = WebHelper.UserHost;
        user.Save();

        return user;
    }

    public static User GetOrAdd(IManageUser user)
    {
        if (user == null) return null;

        var mail = (user as IUser)?.Mail;
        var u = GetOrAdd(user.ID, user.Name, mail);
        if (u != null)
        {
            // 使用SSO资料覆盖本地资料
            if (!user.NickName.IsNullOrEmpty()) u.NickName = user.NickName;
            if (user is IUser au)
            {
                u.Email = au.Mail;

                // 同步管理员
                if (!u.IsAdmin && au.Roles.Any(e => e.Enable && e.IsSystem)) u.IsAdmin = true;

                // 同步密码，使用魔方用户表作为唯一的密码保存地
                if (!u.Password.IsNullOrEmpty())
                {
                    au.Password = u.Password;
                    u.Password = null;

                    (au as IEntity).Update();
                }
            }

            if (u.RegisterTime.Year < 1000) u.RegisterTime = u.CreateTime;

            u.SaveAsync();
        }

        return u;
    }

    public Boolean Login(String ip)
    {
        var user = this;

        user.Logins++;
        user.LastLogin = DateTime.Now;
        user.LastLoginIP = ip;
        user.Save();

        return true;
    }

    public static User Check(String name)
    {
        var user = FindByName(name) ?? FindByEmail(name);
        if (user == null) return null;

        if (!user.Enable) return null;
        //if (user.Password != pass.MD5()) return null;

        return user;
    }
    #endregion
}