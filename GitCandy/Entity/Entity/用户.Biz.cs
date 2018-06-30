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
using System.Security.Principal;
using System.Web;
using NewLife.Data;
using NewLife.Log;
using NewLife.Model;
using NewLife.Web;
using XCode;
using XCode.Membership;

namespace NewLife.GitCandy.Entity
{
    /// <summary>用户</summary>
    public partial class User : LogEntity<User>, IManageUser, IIdentity
    {
        #region 对象操作
        ///// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //protected override void InitData()
        //{
        //    base.InitData();

        //    // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
        //    // Meta.Count是快速取得表记录数
        //    if (Meta.Count > 0) return;

        //    // 需要注意的是，如果该方法调用了其它实体类的首次数据库操作，目标实体类的数据初始化将会在同一个线程完成
        //    if (XTrace.Debug) XTrace.WriteLine("开始初始化{0}[{1}]数据……", typeof(User).Name, Meta.Table.DataTable.DisplayName);

        //    var entity = new User
        //    {
        //        Name = "candy",
        //        NickName = "管理员",
        //        Password = "candy".MD5(),
        //        Email = "admin@newlifex.com",
        //        Enable = true,
        //        IsAdmin = true,
        //        RegisterTime = DateTime.Now
        //    };

        //    entity.Insert();

        //    if (XTrace.Debug) XTrace.WriteLine("完成初始化{0}[{1}]数据！", typeof(User).Name, Meta.Table.DataTable.DisplayName);
        //}

        protected override Int32 OnDelete()
        {
            Teams?.Delete(true);
            UserTeam.FindAllByTeamID(ID).Delete(true);
            Repositories?.Delete(true);
            Repository.FindAllByOwnerID(ID).Delete();
            //AuthorizationLog.FindAllByUserID(ID).Delete();

            return base.OnDelete();
        }
        #endregion

        #region 扩展属性
        private IList<UserTeam> _Teams;
        /// <summary>团队关系</summary>
        public IList<UserTeam> Teams
        {
            get
            {
                if (_Teams == null && !Dirtys.ContainsKey("Teams"))
                {
                    _Teams = UserTeam.FindAllByUserID(ID);

                    Dirtys["Teams"] = true;
                }
                return _Teams;
            }
            set { _Teams = value; }
        }

        public String[] TeamNames { get { return Teams?.Select(e => e.Team?.Name).OrderBy(e => e).ToArray(); } }

        private IList<UserRepository> _Repositories;
        /// <summary>仓库关系</summary>
        public IList<UserRepository> Repositories
        {
            get
            {
                if (_Repositories == null && !Dirtys.ContainsKey("Repositories"))
                {
                    _Repositories = UserRepository.FindAllByUserID(ID);

                    Dirtys["Repositories"] = true;
                }
                return _Repositories;
            }
            set { _Repositories = value; }
        }

        public String[] RepositoryNames { get { return Repositories?.Select(e => e.RepositoryName).ToArray(); } }

        /// <summary>当前登录用户</summary>
        public static User Current
        {
            get
            {
                var ss = HttpContext.Current?.Session;
                if (ss == null) return null;

                return ss["CandyUser"] as User;
            }
            set
            {
                var ss = HttpContext.Current?.Session;
                if (ss == null) return;

                ss["CandyUser"] = value;
            }
        }

        String IIdentity.AuthenticationType => "GitCandy";

        Boolean IIdentity.IsAuthenticated => true;
        #endregion

        #region 扩展查询
        public static User FindByID(Int32 id)
        {
            if (id <= 0) return null;

            if (Meta.Count >= 1000)
                return Find(__.ID, id);
            else // 实体缓存
                return Meta.Cache.Entities.FirstOrDefault(e => e.ID == id);
        }

        /// <summary>根据名称。登录用户名查找</summary>
        /// <param name="name">名称。登录用户名</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static User FindByName(String name)
        {
            if (name.IsNullOrEmpty()) return null;

            if (Meta.Count >= 1000)
                return Find(__.Name, name);
            else // 实体缓存
                return Meta.Cache.Entities.FirstOrDefault(e => e.Name.EqualIgnoreCase(name));
            // 单对象缓存
            //return Meta.SingleCache[name];
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
        public static IList<User> SearchByName(String name, PageParameter param)
        {
            return FindAll(_.IsTeam.IsTrue(false) & _.Name.Contains(name), param);
        }

        public static IList<User> SearchTeam(String name, PageParameter param)
        {
            return FindAll(_.IsTeam == true & _.Name.Contains(name), param);
        }

        /// <summary>搜索用户，不包含团队</summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IList<User> SearchUser(String key, PageParameter param)
        {
            return FindAll(_.IsTeam.IsTrue(false) & (_.Name.Contains(key) | _.Email.Contains(key)), param);
        }
        #endregion

        #region 扩展操作
        public override String ToString()
        {
            return NickName ?? Name;
        }
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
                Password = password.MD5(),
                Enable = true,
                Description = description,
                RegisterTime = DateTime.Now
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
                Description = description,
                RegisterTime = DateTime.Now
            };

            user.Save();

            return user;
        }

        public Boolean Login(String password)
        {
            var user = this;
            if (!user.Enable) return false;

            // 清空密码后，任意密码可以登录，并成为新密码
            if (user.Password.IsNullOrEmpty())
                user.Password = password.MD5();
            else
            {
                if (user.Password != password.MD5()) return false;
            }

            user.Logins++;
            user.LastLogin = DateTime.Now;
            user.LastLoginIP = WebHelper.UserHost;
            user.Save();

            return true;
        }

        public static User Check(String name, String pass)
        {
            var user = FindByName(name) ?? FindByEmail(name);
            if (user == null) return null;

            if (!user.Enable) return null;
            if (user.Password != pass.MD5()) return null;

            return user;
        }
        #endregion
    }
}