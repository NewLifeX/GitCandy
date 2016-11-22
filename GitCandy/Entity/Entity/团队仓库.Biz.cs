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
    /// <summary>团队仓库</summary>
    public partial class TeamRepository : LogEntity<TeamRepository>
    {
        #region 对象操作
        #endregion

        #region 扩展属性
        private Team _Team;
        /// <summary>团队</summary>
        public Team Team
        {
            get
            {
                //if (_Team == null && GatewayID > 0 && !Dirtys.ContainsKey("Team"))
                {
                    _Team = Team.FindByID(TeamID);
                    //Dirtys["Team"] = true;
                }
                return _Team;
            }
            set { _Team = value; }
        }

        /// <summary>团队名称</summary>
        [DisplayName("团队")]
        [Map(__.TeamID, typeof(Team), "ID")]
        public String TeamName { get { return Team + ""; } }

        private Repository _Repository;
        /// <summary>仓库</summary>
        public Repository Repository
        {
            get
            {
                //if (_Repository == null && RepositoryID > 0 && !Dirtys.ContainsKey("Repository"))
                {
                    _Repository = Repository.FindByID(RepositoryID);
                    //Dirtys["Repository"] = true;
                }
                return _Repository;
            }
            set { _Repository = value; }
        }

        /// <summary>仓库名称</summary>
        [DisplayName("仓库")]
        [Map(__.RepositoryID, typeof(Repository), "ID")]
        public String RepositoryName { get { return Repository + ""; } }
        #endregion

        #region 扩展查询
        /// <summary>根据团队、仓库查找</summary>
        /// <param name="teamid">团队</param>
        /// <param name="repositoryid">仓库</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static TeamRepository FindByTeamIDAndRepositoryID(Int32 teamid, Int32 repositoryid)
        {
            if (Meta.Count >= 1000)
                return Find(new String[] { __.TeamID, __.RepositoryID }, new Object[] { teamid, repositoryid });
            else // 实体缓存
                return Meta.Cache.Entities.Find(e => e.TeamID == teamid && e.RepositoryID == repositoryid);
        }

        public static EntityList<TeamRepository> FindAllByRepositoryID(Int32 repid)
        {
            if (repid <= 0) return new EntityList<TeamRepository>();

            if (Meta.Count >= 1000)
                return FindAll(__.RepositoryID, repid);
            else
                return Meta.Cache.Entities.FindAll(e => e.RepositoryID == repid);
        }

        public static EntityList<TeamRepository> FindAllByTeamID(Int32 teamid)
        {
            if (teamid <= 0) return new EntityList<TeamRepository>();

            if (Meta.Count >= 1000)
                return FindAll(__.TeamID, teamid);
            else
                return Meta.Cache.Entities.FindAll(e => e.TeamID == teamid);
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
        public static EntityList<TeamRepository> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
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