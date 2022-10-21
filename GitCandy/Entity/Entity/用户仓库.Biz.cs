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
using NewLife.Data;
using XCode;
using XCode.Membership;

namespace NewLife.GitCandy.Entity
{
    /// <summary>用户仓库</summary>
    public partial class UserRepository : LogEntity<UserRepository>
    {
        #region 对象操作
        static UserRepository()
        {
            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
        }

        public override void Valid(Boolean isNew)
        {
            if (UserID <= 0) throw new ArgumentNullException(__.UserID, _.UserID.DisplayName);
            if (RepositoryID <= 0) throw new ArgumentNullException(__.RepositoryID, _.RepositoryID.DisplayName);

            base.Valid(isNew);
        }
        #endregion

        #region 扩展属性
        /// <summary>团队</summary>
        public User User { get { return Extends.Get(nameof(User), k => User.FindByID(UserID)); } }

        /// <summary>用户名称</summary>
        [DisplayName("用户")]
        [Map(__.UserID, typeof(User), "ID")]
        public String UserName { get { return User + ""; } }

        /// <summary>仓库</summary>
        public Repository Repository { get { return Extends.Get(nameof(Repository), k => Repository.FindByID(RepositoryID)); } }

        /// <summary>仓库名称</summary>
        [DisplayName("仓库")]
        [Map(__.RepositoryID, typeof(Repository), "ID")]
        public String RepositoryName { get { return Repository + ""; } }
        #endregion

        #region 扩展查询

        /// <summary>根据用户、仓库查找</summary>
        /// <param name="userid">用户</param>
        /// <param name="repositoryid">仓库</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static UserRepository FindByUserIDAndRepositoryID(Int32 userid, Int32 repositoryid)
        {
            if (Meta.Count >= 1000)
                return Find(new String[] { __.UserID, __.RepositoryID }, new Object[] { userid, repositoryid });
            else // 实体缓存
                return Meta.Cache.Entities.FirstOrDefault(e => e.UserID == userid && e.RepositoryID == repositoryid);
        }

        public static IList<UserRepository> FindAllByUserID(Int32 userid)
        {
            if (userid <= 0) return new List<UserRepository>();

            if (Meta.Count >= 1000)
                return FindAll(_.UserID == userid);
            else
                return Meta.Cache.Entities.Where(e => e.UserID == userid).ToList();
        }

        public static IList<UserRepository> FindAllByRepositoryID(Int32 repid)
        {
            if (repid <= 0) return new List<UserRepository>();

            if (Meta.Count >= 1000)
                return FindAll(_.RepositoryID == repid);
            else
                return Meta.Cache.Entities.Where(e => e.RepositoryID == repid).ToList();
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
        public static IList<UserRepository> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
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
        public override String ToString() => $"{UserName},{RepositoryName}";
        #endregion
    }
}