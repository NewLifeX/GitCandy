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
using System.Web;
using System.Xml.Linq;
using NewLife.Data;
using NewLife.Model;
using NewLife.Web;
using XCode;
using XCode.Membership;

namespace NewLife.GitCandy.Entity
{
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
                if (_Teams == null && !Dirtys["Teams"])
                {
                    _Teams = UserTeam.FindAllByUserID(ID);

                    Dirtys["Teams"] = true;
                }
                return _Teams;
            }
            set { _Teams = value; }
        }

        public String[] TeamNames => Teams?.Select(e => e.Team?.Name).OrderBy(e => e).ToArray();

        private IList<UserRepository> _Repositories;
        /// <summary>仓库关系</summary>
        public IList<UserRepository> Repositories
        {
            get
            {
                if (_Repositories == null && !Dirtys["Repositories"])
                {
                    _Repositories = UserRepository.FindAllByUserID(ID);

                    Dirtys["Repositories"] = true;
                }
                return _Repositories;
            }
            set { _Repositories = value; }
        }

        public String[] RepositoryNames => Repositories?.Select(e => e.RepositoryName).ToArray();

        ///// <summary>当前登录用户</summary>
        //public static User Current
        //{
        //    get
        //    {
        //        var ss = HttpContext.Current?.Session;
        //        if (ss == null) return null;

        //        return ss["CandyUser"] as User;
        //    }
        //    set
        //    {
        //        var ss = HttpContext.Current?.Session;
        //        if (ss == null) return;

        //        ss["CandyUser"] = value;
        //    }
        //}

        //String IIdentity.AuthenticationType => "GitCandy";

        //Boolean IIdentity.IsAuthenticated => true;
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

        public static User GetOrAdd(Int32 linkid, String name)
        {
            var user = Find(_.LinkID == linkid);
            if (user == null) user = FindByName(name);
            if (user != null)
            {
                if (user.LinkID > 0 && user.LinkID != linkid) throw new InvalidOperationException($"账号[{name}]被[{user.LinkID}]和[{linkid}]共用，请联系管理员");

                user.LinkID = linkid;
                user.SaveAsync();
            }
            else
            {
                user = new User { LinkID = linkid, Name = name };
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

            var u = GetOrAdd(user.ID, user.Name);
            if (u != null)
            {
                u.NickName = user.NickName;
                if (user is XCode.Membership.IUser au) u.Email = au.Mail;
                u.SaveAsync();
            }

            return u;
        }
        #endregion
    }
}