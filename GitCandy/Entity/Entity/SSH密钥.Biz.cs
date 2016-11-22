/*
 * XCoder v6.8.6160.27608
 * 作者：Stone/X2
 * 时间：2016-11-21 15:48:51
 * 版权：版权所有 (C) 新生命开发团队 2002~2016
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using NewLife.Log;
using NewLife.Web;
using NewLife.Data;
using XCode;
using XCode.Configuration;
using XCode.Membership;

namespace NewLife.GitCandy.Entity
{
    /// <summary>SSH密钥</summary>
    public partial class SshKey : LogEntity<SshKey>
    {
        #region 对象操作
        #endregion

        #region 扩展属性
        private User _User;
        /// <summary>团队</summary>
        public User User
        {
            get
            {
                //if (_User == null && GatewayID > 0 && !Dirtys.ContainsKey("User"))
                {
                    _User = User.FindByID(UserID);
                    //Dirtys["User"] = true;
                }
                return _User;
            }
            set { _User = value; }
        }

        /// <summary>用户名称</summary>
        [DisplayName("用户")]
        [Map(__.UserID, typeof(User), "ID")]
        public String UserName { get { return User + ""; } }
        #endregion

        #region 扩展查询

        /// <summary>根据用户查找</summary>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static SshKey FindByUserID(Int32 userid)
        {
            if (userid <= 0) return null;

            if (Meta.Count >= 1000)
                return Find(__.UserID, userid);
            else // 实体缓存
                return Meta.Cache.Entities.Find(__.UserID, userid);
            // 单对象缓存
            //return Meta.SingleCache[userid];
        }

        public static EntityList<SshKey> FindAllByUserID(Int32 userid)
        {
            if (userid <= 0) return new EntityList<SshKey>();

            if (Meta.Count >= 1000)
                return FindAll(__.UserID, userid);
            else
                return Meta.Cache.Entities.FindAll(e => e.UserID == userid);
        }

        public static SshKey FindByFingerprint(String fingerprint)
        {
            return Find(__.Fingerprint, fingerprint);
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
        public static EntityList<SshKey> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
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
        #endregion
    }
}