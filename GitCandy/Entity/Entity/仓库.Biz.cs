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
    /// <summary>仓库</summary>
    public partial class Repository : LogEntity<Repository>
    {
        #region 对象操作
        static Repository()
        {
            var df = Meta.Factory.AdditionalFields;
            df.Add(__.Views);
            df.Add(__.Downloads);

            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
        }

        public override void Valid(Boolean isNew)
        {
            if (OwnerID <= 0) throw new ArgumentNullException(__.OwnerID, _.OwnerID.DisplayName);

            base.Valid(isNew);
        }

        protected override Int32 OnDelete()
        {
            UserRepository.FindAllByRepositoryID(ID).Delete();

            return base.OnDelete();
        }
        #endregion

        #region 扩展属性
        private User _Owner;
        /// <summary>团队</summary>
        public User Owner
        {
            get
            {
                //if (_User == null && UserID > 0 && !Dirtys.ContainsKey("User"))
                {
                    _Owner = User.FindByID(OwnerID);
                    //Dirtys["User"] = true;
                }
                return _Owner;
            }
            set { _Owner = value; }
        }

        /// <summary>用户名称</summary>
        [DisplayName("用户")]
        [Map(__.OwnerID, typeof(User), "ID")]
        public String OwnerName { get { return Owner + ""; } }
        #endregion

        #region 扩展查询
        public static Repository FindByID(Int32 id)
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
        public static Repository FindByOwnerIDAndName(Int32 userid, String name)
        {
            if (userid <= 0 || name.IsNullOrEmpty()) return null;

            if (Meta.Count >= 1000)
                return Find(new String[] { __.OwnerID, __.Name }, new Object[] { userid, name });
            else // 实体缓存
                return Meta.Cache.Entities.ToList().FirstOrDefault(e => e.OwnerID == userid && e.Name == name);
            // 单对象缓存
            //return Meta.SingleCache[name];
        }

        public static Repository FindByOwnerAndName(String owner, String name)
        {
            if (owner.IsNullOrEmpty() || name.IsNullOrEmpty()) return null;

            var user = User.FindByName(owner);
            if (user == null) return null;

            return FindByOwnerIDAndName(user.ID, name);
        }

        public static IList<Repository> FindAllByOwnerID(Int32 userid)
        {
            if (userid <= 0) return new List<Repository>();

            if (Meta.Count >= 1000)
                return FindAll(_.OwnerID == userid);
            else
                return Meta.Cache.Entities.Where(e => e.OwnerID == userid).ToList();
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
        public static IList<Repository> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
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

        public static IList<Repository> GetPublics(PageParameter param = null)
        {
            if (param == null) param = new PageParameter { PageSize = 20 };

            return FindAll(_.Enable == true & _.IsPrivate.IsTrue(false), param);
        }

        public static IList<Repository> Search(Boolean showAll, IEnumerable<Int32> excludes = null, PageParameter param = null)
        {
            if (param == null) param = new PageParameter { PageSize = 20 };

            var exp = _.Enable == true;
            // 只显示共有
            if (!showAll) exp &= _.IsPrivate == false;
            if (excludes != null) exp &= _.ID.NotIn(excludes);

            return FindAll(exp, param);
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        public Boolean CanViewFor(User user)
        {
            if (!Enable) return false;

            // 公开库
            if (!IsPrivate) return true;

            //// 系统管理员
            //if (user.IsAdmin) return true;

            // 个人
            if (OwnerID == user.ID) return true;

            // 团队
            foreach (var team in user.Teams)
            {
                if (team.TeamID == OwnerID) return true;
            }

            return false;
        }

        //public Boolean CanViewFor(Team team)
        //{
        //    // 公开库
        //    if (!IsPrivate) return true;

        //    var tr = TeamRepository.FindByTeamIDAndRepositoryID(team.ID, ID);
        //    if (tr != null && tr.AllowRead) return true;

        //    return false;
        //}
        #endregion
    }
}