using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using XCode;
using XCode.Membership;

namespace NewLife.GitCandy.Entity
{
    public partial class GitHistory : Entity<GitHistory>
    {
        #region 对象操作
        static GitHistory()
        {
            Meta.Table.DataTable.InsertOnly = true;

            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(UserID));
            // 按天分表
            //Meta.ShardPolicy = new TimeShardPolicy(nameof(Id), Meta.Factory)
            //{
            //    TablePolicy = "{{0}}_{{1:yyyyMMdd}}",
            //    Step = TimeSpan.FromDays(1),
            //};

            // 过滤器 UserModule、TimeModule、IPModule
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
            Meta.Modules.Add<TraceModule>();
        }

        /// <summary>验证并修补数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            var len = _.UserAgent.Length;
            if (len > 0 && !UserAgent.IsNullOrEmpty() && UserAgent.Length > len) UserAgent = UserAgent[..len];

            // 建议先调用基类方法，基类方法会做一些统一处理
            base.Valid(isNew);

            // 在新插入数据或者修改了指定字段时进行修正
            //if (isNew && !Dirtys[nameof(CreateTime)]) CreateTime = DateTime.Now;
            //if (isNew && !Dirtys[nameof(CreateIP)]) CreateIP = ManageProvider.UserHost;

            if (isNew && Creator.IsNullOrEmpty()) Creator = Environment.MachineName;

            if (UserID == 0 && Name != null) UserID = (User.FindByName(Name) ?? User.FindByEmail(Name))?.ID ?? 0;
        }

        #endregion

        #region 扩展属性
        /// <summary>用户</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        public User User => Extends.Get(nameof(User), k => User.FindByID(UserID));

        /// <summary>用户</summary>
        [Map(nameof(UserID), typeof(User), "ID")]
        public String UserName => User?.ToString();

        /// <summary>仓库</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        public Repository Repository => Extends.Get(nameof(Repository), k => Repository.FindByID(RepositoryID));

        /// <summary>仓库</summary>
        [Map(nameof(RepositoryID), typeof(Repository), "ID")]
        public String RepositoryName => Repository?.Name;

        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static GitHistory FindById(Int64 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Id == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.Id == id);
        }

        /// <summary>根据用户、操作查找</summary>
        /// <param name="userId">用户</param>
        /// <param name="action">操作</param>
        /// <returns>实体列表</returns>
        public static IList<GitHistory> FindAllByUserIDAndAction(Int32 userId, String action)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.UserID == userId && e.Action.EqualIgnoreCase(action));

            return FindAll(_.UserID == userId & _.Action == action);
        }

        /// <summary>根据仓库、操作查找</summary>
        /// <param name="repositoryId">仓库</param>
        /// <param name="action">操作</param>
        /// <returns>实体列表</returns>
        public static IList<GitHistory> FindAllByRepositoryIDAndAction(Int32 repositoryId, String action)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.RepositoryID == repositoryId && e.Action.EqualIgnoreCase(action));

            return FindAll(_.RepositoryID == repositoryId & _.Action == action);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="userId">用户</param>
        /// <param name="repositoryId">仓库</param>
        /// <param name="action">操作</param>
        /// <param name="start">创建时间开始</param>
        /// <param name="end">创建时间结束</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<GitHistory> Search(Int32 userId, Int32 repositoryId, String action, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (userId >= 0) exp &= _.UserID == userId;
            if (repositoryId >= 0) exp &= _.RepositoryID == repositoryId;
            if (!action.IsNullOrEmpty()) exp &= _.Action == action;
            exp &= _.Id.Between(start, end, Meta.Factory.Snow);
            if (!key.IsNullOrEmpty()) exp &= _.Name.Contains(key) | _.Action.Contains(key) | _.Version.Contains(key) | _.UserAgent.Contains(key) | _.TraceId.Contains(key) | _.Creator.Contains(key) | _.CreateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        // Select Count(Id) as Id,Category From GitHistory Where CreateTime>'2020-01-24 00:00:00' Group By Category Order By Id Desc limit 20
        //static readonly FieldCache<GitHistory> _CategoryCache = new FieldCache<GitHistory>(nameof(Category))
        //{
        //Where = _.CreateTime > DateTime.Today.AddDays(-30) & Expression.Empty
        //};

        ///// <summary>获取类别列表，字段缓存10分钟，分组统计数据最多的前20种，用于魔方前台下拉选择</summary>
        ///// <returns></returns>
        //public static IDictionary<String, String> GetCategoryList() => _CategoryCache.FindAllName();
        #endregion

        #region 业务操作
        #endregion
    }
}