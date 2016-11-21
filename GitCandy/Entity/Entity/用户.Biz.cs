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
using GitCandy.Security;
using NewLife.Data;
using NewLife.Log;
using NewLife.Web;
using XCode;

namespace NewLife.GitCandy.Entity
{
    /// <summary>用户</summary>
    public partial class User : Entity<User>
    {
        #region 对象操作
        /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void InitData()
        {
            base.InitData();

            // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
            // Meta.Count是快速取得表记录数
            if (Meta.Count > 0) return;

            // 需要注意的是，如果该方法调用了其它实体类的首次数据库操作，目标实体类的数据初始化将会在同一个线程完成
            if (XTrace.Debug) XTrace.WriteLine("开始初始化{0}[{1}]数据……", typeof(User).Name, Meta.Table.DataTable.DisplayName);

            var entity = new User();
            entity.Name = "admin";
            entity.Nickname = "管理员";
            entity.Password = "admin".MD5();
            entity.Enable = true;
            entity.IsAdmin = true;
            entity.RegisterTime = DateTime.Now;

            entity.Insert();

            if (XTrace.Debug) XTrace.WriteLine("完成初始化{0}[{1}]数据！", typeof(User).Name, Meta.Table.DataTable.DisplayName);
        }

        protected override Int32 OnDelete()
        {
            UserTeam.FindAllByUserID(ID).Delete();
            UserRepository.FindAllByUserID(ID).Delete();
            AuthorizationLog.FindAllByUserID(ID).Delete();
            SshKey.FindAllByUserID(ID).Delete();

            return base.OnDelete();
        }
        #endregion

        #region 扩展属性
        private List<UserTeam> _Teams;
        /// <summary>绑定信息</summary>
        public List<UserTeam> Teams
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

        public String[] TeamNames { get { return Teams?.Select(e => e.TeamName).OrderBy(e => e).ToArray(); } }

        private List<UserRepository> _Repositories;
        /// <summary>绑定信息</summary>
        public List<UserRepository> Repositories
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

        private List<SshKey> _SshKeys;
        /// <summary>绑定信息</summary>
        public List<SshKey> SshKeys
        {
            get
            {
                if (_SshKeys == null && !Dirtys.ContainsKey("SshKeys"))
                {
                    _SshKeys = SshKey.FindAllByUserID(ID);

                    Dirtys["SshKeys"] = true;
                }
                return _SshKeys;
            }
            set { _SshKeys = value; }
        }
        #endregion

        #region 扩展查询
        public static User FindByID(Int32 id)
        {
            if (id <= 0) return null;

            if (Meta.Count >= 1000)
                return Find(__.ID, id);
            else // 实体缓存
                return Meta.Cache.Entities.Find(__.ID, id);
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
                return Meta.Cache.Entities.Find(__.Name, name);
            // 单对象缓存
            //return Meta.SingleCache[name];
        }

        public static User FindByEmail(String email)
        {
            if (email.IsNullOrEmpty()) return null;

            if (Meta.Count >= 1000)
                return Find(__.Email, email);
            else // 实体缓存
                return Meta.Cache.Entities.Find(__.Email, email);
            // 单对象缓存
            //return Meta.SingleCache[name];
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
        public static EntityList<User> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
        {
            // WhereExpression重载&和|运算符，作为And和Or的替代
            // SearchWhereByKeys系列方法用于构建针对字符串字段的模糊搜索，第二个参数可指定要搜索的字段
            var exp = SearchWhereByKeys(key, null, null);

            // 以下仅为演示，Field（继承自FieldItem）重载了==、!=、>、<、>=、<=等运算符
            //if (userid > 0) exp &= _.OperatorID == userid;
            //if (isSign != null) exp &= _.IsSign == isSign.Value;
            //exp &= _.OccurTime.Between(start, end); // 大于等于start，小于end，当start/end大于MinValue时有效

            return FindAll(exp, param);
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        public Boolean Login(String password)
        {
            var user = this;
            if (user.Password != password.MD5()) return false;

            user.Logins++;
            user.LastLogin = DateTime.Now;
            user.LastLoginIP = WebHelper.UserHost;
            user.Save();

            return true;
        }
        #endregion
    }
}