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
    /// <summary>仓库</summary>
    [Serializable]
    [DataObject]
    [Description("仓库")]
    [BindIndex("IU_Repository_OwnerID_Name", true, "OwnerID,Name")]
    [BindIndex("IX_Repository_Name", false, "Name")]
    [BindTable("Repository", Description = "仓库", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class Repository
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("ID", "编号", "")]
        public Int32 ID { get => _ID; set { if (OnPropertyChanging("ID", value)) { _ID = value; OnPropertyChanged("ID"); } } }

        private Int32 _OwnerID;
        /// <summary>拥有者</summary>
        [DisplayName("拥有者")]
        [Description("拥有者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("OwnerID", "拥有者", "")]
        public Int32 OwnerID { get => _OwnerID; set { if (OnPropertyChanging("OwnerID", value)) { _OwnerID = value; OnPropertyChanged("OwnerID"); } } }

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称", "", Master = true)]
        public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Enable", "启用", "")]
        public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

        private Boolean _IsPrivate;
        /// <summary>私有</summary>
        [DisplayName("私有")]
        [Description("私有")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsPrivate", "私有", "")]
        public Boolean IsPrivate { get => _IsPrivate; set { if (OnPropertyChanging("IsPrivate", value)) { _IsPrivate = value; OnPropertyChanged("IsPrivate"); } } }

        private Boolean _AllowAnonymousRead;
        /// <summary>匿名读</summary>
        [DisplayName("匿名读")]
        [Description("匿名读")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("AllowAnonymousRead", "匿名读", "")]
        public Boolean AllowAnonymousRead { get => _AllowAnonymousRead; set { if (OnPropertyChanging("AllowAnonymousRead", value)) { _AllowAnonymousRead = value; OnPropertyChanged("AllowAnonymousRead"); } } }

        private Boolean _AllowAnonymousWrite;
        /// <summary>匿名写</summary>
        [DisplayName("匿名写")]
        [Description("匿名写")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("AllowAnonymousWrite", "匿名写", "")]
        public Boolean AllowAnonymousWrite { get => _AllowAnonymousWrite; set { if (OnPropertyChanging("AllowAnonymousWrite", value)) { _AllowAnonymousWrite = value; OnPropertyChanged("AllowAnonymousWrite"); } } }

        private Int32 _Commits;
        /// <summary>提交数</summary>
        [DisplayName("提交数")]
        [Description("提交数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Commits", "提交数", "")]
        public Int32 Commits { get => _Commits; set { if (OnPropertyChanging("Commits", value)) { _Commits = value; OnPropertyChanged("Commits"); } } }

        private Int32 _Branches;
        /// <summary>分支数</summary>
        [DisplayName("分支数")]
        [Description("分支数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Branches", "分支数", "")]
        public Int32 Branches { get => _Branches; set { if (OnPropertyChanging("Branches", value)) { _Branches = value; OnPropertyChanged("Branches"); } } }

        private Int32 _Contributors;
        /// <summary>参与者</summary>
        [DisplayName("参与者")]
        [Description("参与者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Contributors", "参与者", "")]
        public Int32 Contributors { get => _Contributors; set { if (OnPropertyChanging("Contributors", value)) { _Contributors = value; OnPropertyChanged("Contributors"); } } }

        private Int32 _Files;
        /// <summary>文件数</summary>
        [DisplayName("文件数")]
        [Description("文件数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Files", "文件数", "")]
        public Int32 Files { get => _Files; set { if (OnPropertyChanging("Files", value)) { _Files = value; OnPropertyChanged("Files"); } } }

        private Int64 _Size;
        /// <summary>源码大小</summary>
        [DisplayName("源码大小")]
        [Description("源码大小")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Size", "源码大小", "")]
        public Int64 Size { get => _Size; set { if (OnPropertyChanging("Size", value)) { _Size = value; OnPropertyChanged("Size"); } } }

        private DateTime _LastCommit;
        /// <summary>最后提交</summary>
        [DisplayName("最后提交")]
        [Description("最后提交")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastCommit", "最后提交", "")]
        public DateTime LastCommit { get => _LastCommit; set { if (OnPropertyChanging("LastCommit", value)) { _LastCommit = value; OnPropertyChanged("LastCommit"); } } }

        private Int32 _Views;
        /// <summary>浏览数</summary>
        [DisplayName("浏览数")]
        [Description("浏览数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Views", "浏览数", "")]
        public Int32 Views { get => _Views; set { if (OnPropertyChanging("Views", value)) { _Views = value; OnPropertyChanged("Views"); } } }

        private Int32 _Downloads;
        /// <summary>下载数</summary>
        [DisplayName("下载数")]
        [Description("下载数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Downloads", "下载数", "")]
        public Int32 Downloads { get => _Downloads; set { if (OnPropertyChanging("Downloads", value)) { _Downloads = value; OnPropertyChanged("Downloads"); } } }

        private DateTime _LastView;
        /// <summary>最后浏览</summary>
        [DisplayName("最后浏览")]
        [Description("最后浏览")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastView", "最后浏览", "")]
        public DateTime LastView { get => _LastView; set { if (OnPropertyChanging("LastView", value)) { _LastView = value; OnPropertyChanged("LastView"); } } }

        private String _Description;
        /// <summary>描述</summary>
        [DisplayName("描述")]
        [Description("描述")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn("Description", "描述", "")]
        public String Description { get => _Description; set { if (OnPropertyChanging("Description", value)) { _Description = value; OnPropertyChanged("Description"); } } }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("CreateUserID", "创建者", "")]
        public Int32 CreateUserID { get => _CreateUserID; set { if (OnPropertyChanging("CreateUserID", value)) { _CreateUserID = value; OnPropertyChanged("CreateUserID"); } } }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("CreateTime", "创建时间", "")]
        public DateTime CreateTime { get => _CreateTime; set { if (OnPropertyChanging("CreateTime", value)) { _CreateTime = value; OnPropertyChanged("CreateTime"); } } }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "")]
        public String CreateIP { get => _CreateIP; set { if (OnPropertyChanging("CreateIP", value)) { _CreateIP = value; OnPropertyChanged("CreateIP"); } } }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("UpdateUserID", "更新者", "")]
        public Int32 UpdateUserID { get => _UpdateUserID; set { if (OnPropertyChanging("UpdateUserID", value)) { _UpdateUserID = value; OnPropertyChanged("UpdateUserID"); } } }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("UpdateTime", "更新时间", "")]
        public DateTime UpdateTime { get => _UpdateTime; set { if (OnPropertyChanging("UpdateTime", value)) { _UpdateTime = value; OnPropertyChanged("UpdateTime"); } } }

        private String _UpdateIP;
        /// <summary>更新地址</summary>
        [DisplayName("更新地址")]
        [Description("更新地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateIP", "更新地址", "")]
        public String UpdateIP { get => _UpdateIP; set { if (OnPropertyChanging("UpdateIP", value)) { _UpdateIP = value; OnPropertyChanged("UpdateIP"); } } }
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
                    case "ID": return _ID;
                    case "OwnerID": return _OwnerID;
                    case "Name": return _Name;
                    case "Enable": return _Enable;
                    case "IsPrivate": return _IsPrivate;
                    case "AllowAnonymousRead": return _AllowAnonymousRead;
                    case "AllowAnonymousWrite": return _AllowAnonymousWrite;
                    case "Commits": return _Commits;
                    case "Branches": return _Branches;
                    case "Contributors": return _Contributors;
                    case "Files": return _Files;
                    case "Size": return _Size;
                    case "LastCommit": return _LastCommit;
                    case "Views": return _Views;
                    case "Downloads": return _Downloads;
                    case "LastView": return _LastView;
                    case "Description": return _Description;
                    case "CreateUserID": return _CreateUserID;
                    case "CreateTime": return _CreateTime;
                    case "CreateIP": return _CreateIP;
                    case "UpdateUserID": return _UpdateUserID;
                    case "UpdateTime": return _UpdateTime;
                    case "UpdateIP": return _UpdateIP;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "ID": _ID = value.ToInt(); break;
                    case "OwnerID": _OwnerID = value.ToInt(); break;
                    case "Name": _Name = Convert.ToString(value); break;
                    case "Enable": _Enable = value.ToBoolean(); break;
                    case "IsPrivate": _IsPrivate = value.ToBoolean(); break;
                    case "AllowAnonymousRead": _AllowAnonymousRead = value.ToBoolean(); break;
                    case "AllowAnonymousWrite": _AllowAnonymousWrite = value.ToBoolean(); break;
                    case "Commits": _Commits = value.ToInt(); break;
                    case "Branches": _Branches = value.ToInt(); break;
                    case "Contributors": _Contributors = value.ToInt(); break;
                    case "Files": _Files = value.ToInt(); break;
                    case "Size": _Size = value.ToLong(); break;
                    case "LastCommit": _LastCommit = value.ToDateTime(); break;
                    case "Views": _Views = value.ToInt(); break;
                    case "Downloads": _Downloads = value.ToInt(); break;
                    case "LastView": _LastView = value.ToDateTime(); break;
                    case "Description": _Description = Convert.ToString(value); break;
                    case "CreateUserID": _CreateUserID = value.ToInt(); break;
                    case "CreateTime": _CreateTime = value.ToDateTime(); break;
                    case "CreateIP": _CreateIP = Convert.ToString(value); break;
                    case "UpdateUserID": _UpdateUserID = value.ToInt(); break;
                    case "UpdateTime": _UpdateTime = value.ToDateTime(); break;
                    case "UpdateIP": _UpdateIP = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得仓库字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field ID = FindByName("ID");

            /// <summary>拥有者</summary>
            public static readonly Field OwnerID = FindByName("OwnerID");

            /// <summary>名称</summary>
            public static readonly Field Name = FindByName("Name");

            /// <summary>启用</summary>
            public static readonly Field Enable = FindByName("Enable");

            /// <summary>私有</summary>
            public static readonly Field IsPrivate = FindByName("IsPrivate");

            /// <summary>匿名读</summary>
            public static readonly Field AllowAnonymousRead = FindByName("AllowAnonymousRead");

            /// <summary>匿名写</summary>
            public static readonly Field AllowAnonymousWrite = FindByName("AllowAnonymousWrite");

            /// <summary>提交数</summary>
            public static readonly Field Commits = FindByName("Commits");

            /// <summary>分支数</summary>
            public static readonly Field Branches = FindByName("Branches");

            /// <summary>参与者</summary>
            public static readonly Field Contributors = FindByName("Contributors");

            /// <summary>文件数</summary>
            public static readonly Field Files = FindByName("Files");

            /// <summary>源码大小</summary>
            public static readonly Field Size = FindByName("Size");

            /// <summary>最后提交</summary>
            public static readonly Field LastCommit = FindByName("LastCommit");

            /// <summary>浏览数</summary>
            public static readonly Field Views = FindByName("Views");

            /// <summary>下载数</summary>
            public static readonly Field Downloads = FindByName("Downloads");

            /// <summary>最后浏览</summary>
            public static readonly Field LastView = FindByName("LastView");

            /// <summary>描述</summary>
            public static readonly Field Description = FindByName("Description");

            /// <summary>创建者</summary>
            public static readonly Field CreateUserID = FindByName("CreateUserID");

            /// <summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName("CreateTime");

            /// <summary>创建地址</summary>
            public static readonly Field CreateIP = FindByName("CreateIP");

            /// <summary>更新者</summary>
            public static readonly Field UpdateUserID = FindByName("UpdateUserID");

            /// <summary>更新时间</summary>
            public static readonly Field UpdateTime = FindByName("UpdateTime");

            /// <summary>更新地址</summary>
            public static readonly Field UpdateIP = FindByName("UpdateIP");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得仓库字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>编号</summary>
            public const String ID = "ID";

            /// <summary>拥有者</summary>
            public const String OwnerID = "OwnerID";

            /// <summary>名称</summary>
            public const String Name = "Name";

            /// <summary>启用</summary>
            public const String Enable = "Enable";

            /// <summary>私有</summary>
            public const String IsPrivate = "IsPrivate";

            /// <summary>匿名读</summary>
            public const String AllowAnonymousRead = "AllowAnonymousRead";

            /// <summary>匿名写</summary>
            public const String AllowAnonymousWrite = "AllowAnonymousWrite";

            /// <summary>提交数</summary>
            public const String Commits = "Commits";

            /// <summary>分支数</summary>
            public const String Branches = "Branches";

            /// <summary>参与者</summary>
            public const String Contributors = "Contributors";

            /// <summary>文件数</summary>
            public const String Files = "Files";

            /// <summary>源码大小</summary>
            public const String Size = "Size";

            /// <summary>最后提交</summary>
            public const String LastCommit = "LastCommit";

            /// <summary>浏览数</summary>
            public const String Views = "Views";

            /// <summary>下载数</summary>
            public const String Downloads = "Downloads";

            /// <summary>最后浏览</summary>
            public const String LastView = "LastView";

            /// <summary>描述</summary>
            public const String Description = "Description";

            /// <summary>创建者</summary>
            public const String CreateUserID = "CreateUserID";

            /// <summary>创建时间</summary>
            public const String CreateTime = "CreateTime";

            /// <summary>创建地址</summary>
            public const String CreateIP = "CreateIP";

            /// <summary>更新者</summary>
            public const String UpdateUserID = "UpdateUserID";

            /// <summary>更新时间</summary>
            public const String UpdateTime = "UpdateTime";

            /// <summary>更新地址</summary>
            public const String UpdateIP = "UpdateIP";
        }
        #endregion
    }
}