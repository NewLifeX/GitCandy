using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.GitCandy.Entity
{
    /// <summary>Git历史</summary>
    [Serializable]
    [DataObject]
    [Description("Git历史")]
    [BindIndex("IX_GitHistory_UserID_Action", false, "UserID,Action")]
    [BindIndex("IX_GitHistory_RepositoryID_Action", false, "RepositoryID,Action")]
    [BindTable("GitHistory", Description = "Git历史", ConnName = "GitCandy", DbType = DatabaseType.None)]
    public partial class GitHistory
    {
        #region 属性
        private Int64 _Id;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, false, false, 0)]
        [BindColumn("Id", "编号", "")]
        public Int64 Id { get => _Id; set { if (OnPropertyChanging("Id", value)) { _Id = value; OnPropertyChanged("Id"); } } }

        private Int32 _UserID;
        /// <summary>用户</summary>
        [DisplayName("用户")]
        [Description("用户")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("UserID", "用户", "")]
        public Int32 UserID { get => _UserID; set { if (OnPropertyChanging("UserID", value)) { _UserID = value; OnPropertyChanged("UserID"); } } }

        private Int32 _RepositoryID;
        /// <summary>仓库</summary>
        [DisplayName("仓库")]
        [Description("仓库")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("RepositoryID", "仓库", "")]
        public Int32 RepositoryID { get => _RepositoryID; set { if (OnPropertyChanging("RepositoryID", value)) { _RepositoryID = value; OnPropertyChanged("RepositoryID"); } } }

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称", "", Master = true)]
        public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

        private String _Action;
        /// <summary>操作</summary>
        [DisplayName("操作")]
        [Description("操作")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Action", "操作", "")]
        public String Action { get => _Action; set { if (OnPropertyChanging("Action", value)) { _Action = value; OnPropertyChanged("Action"); } } }

        private Boolean _Success;
        /// <summary>成功</summary>
        [DisplayName("成功")]
        [Description("成功")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Success", "成功", "")]
        public Boolean Success { get => _Success; set { if (OnPropertyChanging("Success", value)) { _Success = value; OnPropertyChanged("Success"); } } }

        private String _Version;
        /// <summary>版本</summary>
        [DisplayName("版本")]
        [Description("版本")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Version", "版本", "")]
        public String Version { get => _Version; set { if (OnPropertyChanging("Version", value)) { _Version = value; OnPropertyChanged("Version"); } } }

        private String _UserAgent;
        /// <summary>客户端</summary>
        [DisplayName("客户端")]
        [Description("客户端")]
        [DataObjectField(false, false, true, 200)]
        [BindColumn("UserAgent", "客户端", "")]
        public String UserAgent { get => _UserAgent; set { if (OnPropertyChanging("UserAgent", value)) { _UserAgent = value; OnPropertyChanged("UserAgent"); } } }

        private String _TraceId;
        /// <summary>追踪。最新一次查看采样，可用于关联多个片段，建立依赖关系，随线程上下文、Http、Rpc传递</summary>
        [Category("扩展")]
        [DisplayName("追踪")]
        [Description("追踪。最新一次查看采样，可用于关联多个片段，建立依赖关系，随线程上下文、Http、Rpc传递")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("TraceId", "追踪。最新一次查看采样，可用于关联多个片段，建立依赖关系，随线程上下文、Http、Rpc传递", "")]
        public String TraceId { get => _TraceId; set { if (OnPropertyChanging("TraceId", value)) { _TraceId = value; OnPropertyChanged("TraceId"); } } }

        private String _Creator;
        /// <summary>创建者。服务端节点</summary>
        [Category("扩展")]
        [DisplayName("创建者")]
        [Description("创建者。服务端节点")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Creator", "创建者。服务端节点", "")]
        public String Creator { get => _Creator; set { if (OnPropertyChanging("Creator", value)) { _Creator = value; OnPropertyChanged("Creator"); } } }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [Category("扩展")]
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("CreateTime", "创建时间", "")]
        public DateTime CreateTime { get => _CreateTime; set { if (OnPropertyChanging("CreateTime", value)) { _CreateTime = value; OnPropertyChanged("CreateTime"); } } }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [Category("扩展")]
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "")]
        public String CreateIP { get => _CreateIP; set { if (OnPropertyChanging("CreateIP", value)) { _CreateIP = value; OnPropertyChanged("CreateIP"); } } }

        private String _Remark;
        /// <summary>内容</summary>
        [DisplayName("内容")]
        [Description("内容")]
        [DataObjectField(false, false, true, 2000)]
        [BindColumn("Content", "内容", "")]
        public String Remark { get => _Remark; set { if (OnPropertyChanging("Remark", value)) { _Remark = value; OnPropertyChanged("Remark"); } } }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case "Id": return _Id;
                    case "UserID": return _UserID;
                    case "RepositoryID": return _RepositoryID;
                    case "Name": return _Name;
                    case "Action": return _Action;
                    case "Success": return _Success;
                    case "Version": return _Version;
                    case "UserAgent": return _UserAgent;
                    case "TraceId": return _TraceId;
                    case "Creator": return _Creator;
                    case "CreateTime": return _CreateTime;
                    case "CreateIP": return _CreateIP;
                    case "Remark": return _Remark;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "Id": _Id = value.ToLong(); break;
                    case "UserID": _UserID = value.ToInt(); break;
                    case "RepositoryID": _RepositoryID = value.ToInt(); break;
                    case "Name": _Name = Convert.ToString(value); break;
                    case "Action": _Action = Convert.ToString(value); break;
                    case "Success": _Success = value.ToBoolean(); break;
                    case "Version": _Version = Convert.ToString(value); break;
                    case "UserAgent": _UserAgent = Convert.ToString(value); break;
                    case "TraceId": _TraceId = Convert.ToString(value); break;
                    case "Creator": _Creator = Convert.ToString(value); break;
                    case "CreateTime": _CreateTime = value.ToDateTime(); break;
                    case "CreateIP": _CreateIP = Convert.ToString(value); break;
                    case "Remark": _Remark = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得Git历史字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field Id = FindByName("Id");

            /// <summary>用户</summary>
            public static readonly Field UserID = FindByName("UserID");

            /// <summary>仓库</summary>
            public static readonly Field RepositoryID = FindByName("RepositoryID");

            /// <summary>名称</summary>
            public static readonly Field Name = FindByName("Name");

            /// <summary>操作</summary>
            public static readonly Field Action = FindByName("Action");

            /// <summary>成功</summary>
            public static readonly Field Success = FindByName("Success");

            /// <summary>版本</summary>
            public static readonly Field Version = FindByName("Version");

            /// <summary>客户端</summary>
            public static readonly Field UserAgent = FindByName("UserAgent");

            /// <summary>追踪。最新一次查看采样，可用于关联多个片段，建立依赖关系，随线程上下文、Http、Rpc传递</summary>
            public static readonly Field TraceId = FindByName("TraceId");

            /// <summary>创建者。服务端节点</summary>
            public static readonly Field Creator = FindByName("Creator");

            /// <summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName("CreateTime");

            /// <summary>创建地址</summary>
            public static readonly Field CreateIP = FindByName("CreateIP");

            /// <summary>内容</summary>
            public static readonly Field Remark = FindByName("Remark");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得Git历史字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>编号</summary>
            public const String Id = "Id";

            /// <summary>用户</summary>
            public const String UserID = "UserID";

            /// <summary>仓库</summary>
            public const String RepositoryID = "RepositoryID";

            /// <summary>名称</summary>
            public const String Name = "Name";

            /// <summary>操作</summary>
            public const String Action = "Action";

            /// <summary>成功</summary>
            public const String Success = "Success";

            /// <summary>版本</summary>
            public const String Version = "Version";

            /// <summary>客户端</summary>
            public const String UserAgent = "UserAgent";

            /// <summary>追踪。最新一次查看采样，可用于关联多个片段，建立依赖关系，随线程上下文、Http、Rpc传递</summary>
            public const String TraceId = "TraceId";

            /// <summary>创建者。服务端节点</summary>
            public const String Creator = "Creator";

            /// <summary>创建时间</summary>
            public const String CreateTime = "CreateTime";

            /// <summary>创建地址</summary>
            public const String CreateIP = "CreateIP";

            /// <summary>内容</summary>
            public const String Remark = "Remark";
        }
        #endregion
    }
}