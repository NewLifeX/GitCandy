using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Repository : IRepository
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("ID", "编号", "int")]
        public Int32 ID { get { return _ID; } set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } } }

        private Int32 _OwnerID;
        /// <summary>拥有者</summary>
        [DisplayName("拥有者")]
        [Description("拥有者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("OwnerID", "拥有者", "int")]
        public Int32 OwnerID { get { return _OwnerID; } set { if (OnPropertyChanging(__.OwnerID, value)) { _OwnerID = value; OnPropertyChanged(__.OwnerID); } } }

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称", "nvarchar(50)", Master = true)]
        public String Name { get { return _Name; } set { if (OnPropertyChanging(__.Name, value)) { _Name = value; OnPropertyChanged(__.Name); } } }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Enable", "启用", "bit")]
        public Boolean Enable { get { return _Enable; } set { if (OnPropertyChanging(__.Enable, value)) { _Enable = value; OnPropertyChanged(__.Enable); } } }

        private Boolean _IsPrivate;
        /// <summary>私有</summary>
        [DisplayName("私有")]
        [Description("私有")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsPrivate", "私有", "bit")]
        public Boolean IsPrivate { get { return _IsPrivate; } set { if (OnPropertyChanging(__.IsPrivate, value)) { _IsPrivate = value; OnPropertyChanged(__.IsPrivate); } } }

        private Boolean _AllowAnonymousRead;
        /// <summary>匿名读</summary>
        [DisplayName("匿名读")]
        [Description("匿名读")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("AllowAnonymousRead", "匿名读", "bit")]
        public Boolean AllowAnonymousRead { get { return _AllowAnonymousRead; } set { if (OnPropertyChanging(__.AllowAnonymousRead, value)) { _AllowAnonymousRead = value; OnPropertyChanged(__.AllowAnonymousRead); } } }

        private Boolean _AllowAnonymousWrite;
        /// <summary>匿名写</summary>
        [DisplayName("匿名写")]
        [Description("匿名写")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("AllowAnonymousWrite", "匿名写", "bit")]
        public Boolean AllowAnonymousWrite { get { return _AllowAnonymousWrite; } set { if (OnPropertyChanging(__.AllowAnonymousWrite, value)) { _AllowAnonymousWrite = value; OnPropertyChanged(__.AllowAnonymousWrite); } } }

        private Int32 _Commits;
        /// <summary>提交数</summary>
        [DisplayName("提交数")]
        [Description("提交数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Commits", "提交数", "int")]
        public Int32 Commits { get { return _Commits; } set { if (OnPropertyChanging(__.Commits, value)) { _Commits = value; OnPropertyChanged(__.Commits); } } }

        private Int32 _Branches;
        /// <summary>分支数</summary>
        [DisplayName("分支数")]
        [Description("分支数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Branches", "分支数", "int")]
        public Int32 Branches { get { return _Branches; } set { if (OnPropertyChanging(__.Branches, value)) { _Branches = value; OnPropertyChanged(__.Branches); } } }

        private Int32 _Contributors;
        /// <summary>参与者</summary>
        [DisplayName("参与者")]
        [Description("参与者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Contributors", "参与者", "int")]
        public Int32 Contributors { get { return _Contributors; } set { if (OnPropertyChanging(__.Contributors, value)) { _Contributors = value; OnPropertyChanged(__.Contributors); } } }

        private Int32 _Files;
        /// <summary>文件数</summary>
        [DisplayName("文件数")]
        [Description("文件数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Files", "文件数", "int")]
        public Int32 Files { get { return _Files; } set { if (OnPropertyChanging(__.Files, value)) { _Files = value; OnPropertyChanged(__.Files); } } }

        private Int64 _Size;
        /// <summary>源码大小</summary>
        [DisplayName("源码大小")]
        [Description("源码大小")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Size", "源码大小", "bigint")]
        public Int64 Size { get { return _Size; } set { if (OnPropertyChanging(__.Size, value)) { _Size = value; OnPropertyChanged(__.Size); } } }

        private DateTime _LastCommit;
        /// <summary>最后提交</summary>
        [DisplayName("最后提交")]
        [Description("最后提交")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastCommit", "最后提交", "datetime")]
        public DateTime LastCommit { get { return _LastCommit; } set { if (OnPropertyChanging(__.LastCommit, value)) { _LastCommit = value; OnPropertyChanged(__.LastCommit); } } }

        private Int32 _Views;
        /// <summary>浏览数</summary>
        [DisplayName("浏览数")]
        [Description("浏览数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Views", "浏览数", "int")]
        public Int32 Views { get { return _Views; } set { if (OnPropertyChanging(__.Views, value)) { _Views = value; OnPropertyChanged(__.Views); } } }

        private Int32 _Downloads;
        /// <summary>下载数</summary>
        [DisplayName("下载数")]
        [Description("下载数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Downloads", "下载数", "int")]
        public Int32 Downloads { get { return _Downloads; } set { if (OnPropertyChanging(__.Downloads, value)) { _Downloads = value; OnPropertyChanged(__.Downloads); } } }

        private DateTime _LastView;
        /// <summary>最后浏览</summary>
        [DisplayName("最后浏览")]
        [Description("最后浏览")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastView", "最后浏览", "datetime")]
        public DateTime LastView { get { return _LastView; } set { if (OnPropertyChanging(__.LastView, value)) { _LastView = value; OnPropertyChanged(__.LastView); } } }

        private String _Description;
        /// <summary>描述</summary>
        [DisplayName("描述")]
        [Description("描述")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn("Description", "描述", "nvarchar(500)")]
        public String Description { get { return _Description; } set { if (OnPropertyChanging(__.Description, value)) { _Description = value; OnPropertyChanged(__.Description); } } }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("CreateUserID", "创建者", "int")]
        public Int32 CreateUserID { get { return _CreateUserID; } set { if (OnPropertyChanging(__.CreateUserID, value)) { _CreateUserID = value; OnPropertyChanged(__.CreateUserID); } } }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("CreateTime", "创建时间", "datetime")]
        public DateTime CreateTime { get { return _CreateTime; } set { if (OnPropertyChanging(__.CreateTime, value)) { _CreateTime = value; OnPropertyChanged(__.CreateTime); } } }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "nvarchar(50)")]
        public String CreateIP { get { return _CreateIP; } set { if (OnPropertyChanging(__.CreateIP, value)) { _CreateIP = value; OnPropertyChanged(__.CreateIP); } } }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("UpdateUserID", "更新者", "int")]
        public Int32 UpdateUserID { get { return _UpdateUserID; } set { if (OnPropertyChanging(__.UpdateUserID, value)) { _UpdateUserID = value; OnPropertyChanged(__.UpdateUserID); } } }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("UpdateTime", "更新时间", "datetime")]
        public DateTime UpdateTime { get { return _UpdateTime; } set { if (OnPropertyChanging(__.UpdateTime, value)) { _UpdateTime = value; OnPropertyChanged(__.UpdateTime); } } }

        private String _UpdateIP;
        /// <summary>更新地址</summary>
        [DisplayName("更新地址")]
        [Description("更新地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateIP", "更新地址", "nvarchar(50)")]
        public String UpdateIP { get { return _UpdateIP; } set { if (OnPropertyChanging(__.UpdateIP, value)) { _UpdateIP = value; OnPropertyChanged(__.UpdateIP); } } }
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
                    case __.ID : return _ID;
                    case __.OwnerID : return _OwnerID;
                    case __.Name : return _Name;
                    case __.Enable : return _Enable;
                    case __.IsPrivate : return _IsPrivate;
                    case __.AllowAnonymousRead : return _AllowAnonymousRead;
                    case __.AllowAnonymousWrite : return _AllowAnonymousWrite;
                    case __.Commits : return _Commits;
                    case __.Branches : return _Branches;
                    case __.Contributors : return _Contributors;
                    case __.Files : return _Files;
                    case __.Size : return _Size;
                    case __.LastCommit : return _LastCommit;
                    case __.Views : return _Views;
                    case __.Downloads : return _Downloads;
                    case __.LastView : return _LastView;
                    case __.Description : return _Description;
                    case __.CreateUserID : return _CreateUserID;
                    case __.CreateTime : return _CreateTime;
                    case __.CreateIP : return _CreateIP;
                    case __.UpdateUserID : return _UpdateUserID;
                    case __.UpdateTime : return _UpdateTime;
                    case __.UpdateIP : return _UpdateIP;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.ID : _ID = Convert.ToInt32(value); break;
                    case __.OwnerID : _OwnerID = Convert.ToInt32(value); break;
                    case __.Name : _Name = Convert.ToString(value); break;
                    case __.Enable : _Enable = Convert.ToBoolean(value); break;
                    case __.IsPrivate : _IsPrivate = Convert.ToBoolean(value); break;
                    case __.AllowAnonymousRead : _AllowAnonymousRead = Convert.ToBoolean(value); break;
                    case __.AllowAnonymousWrite : _AllowAnonymousWrite = Convert.ToBoolean(value); break;
                    case __.Commits : _Commits = Convert.ToInt32(value); break;
                    case __.Branches : _Branches = Convert.ToInt32(value); break;
                    case __.Contributors : _Contributors = Convert.ToInt32(value); break;
                    case __.Files : _Files = Convert.ToInt32(value); break;
                    case __.Size : _Size = Convert.ToInt64(value); break;
                    case __.LastCommit : _LastCommit = Convert.ToDateTime(value); break;
                    case __.Views : _Views = Convert.ToInt32(value); break;
                    case __.Downloads : _Downloads = Convert.ToInt32(value); break;
                    case __.LastView : _LastView = Convert.ToDateTime(value); break;
                    case __.Description : _Description = Convert.ToString(value); break;
                    case __.CreateUserID : _CreateUserID = Convert.ToInt32(value); break;
                    case __.CreateTime : _CreateTime = Convert.ToDateTime(value); break;
                    case __.CreateIP : _CreateIP = Convert.ToString(value); break;
                    case __.UpdateUserID : _UpdateUserID = Convert.ToInt32(value); break;
                    case __.UpdateTime : _UpdateTime = Convert.ToDateTime(value); break;
                    case __.UpdateIP : _UpdateIP = Convert.ToString(value); break;
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
            public static readonly Field ID = FindByName(__.ID);

            /// <summary>拥有者</summary>
            public static readonly Field OwnerID = FindByName(__.OwnerID);

            /// <summary>名称</summary>
            public static readonly Field Name = FindByName(__.Name);

            /// <summary>启用</summary>
            public static readonly Field Enable = FindByName(__.Enable);

            /// <summary>私有</summary>
            public static readonly Field IsPrivate = FindByName(__.IsPrivate);

            /// <summary>匿名读</summary>
            public static readonly Field AllowAnonymousRead = FindByName(__.AllowAnonymousRead);

            /// <summary>匿名写</summary>
            public static readonly Field AllowAnonymousWrite = FindByName(__.AllowAnonymousWrite);

            /// <summary>提交数</summary>
            public static readonly Field Commits = FindByName(__.Commits);

            /// <summary>分支数</summary>
            public static readonly Field Branches = FindByName(__.Branches);

            /// <summary>参与者</summary>
            public static readonly Field Contributors = FindByName(__.Contributors);

            /// <summary>文件数</summary>
            public static readonly Field Files = FindByName(__.Files);

            /// <summary>源码大小</summary>
            public static readonly Field Size = FindByName(__.Size);

            /// <summary>最后提交</summary>
            public static readonly Field LastCommit = FindByName(__.LastCommit);

            /// <summary>浏览数</summary>
            public static readonly Field Views = FindByName(__.Views);

            /// <summary>下载数</summary>
            public static readonly Field Downloads = FindByName(__.Downloads);

            /// <summary>最后浏览</summary>
            public static readonly Field LastView = FindByName(__.LastView);

            /// <summary>描述</summary>
            public static readonly Field Description = FindByName(__.Description);

            /// <summary>创建者</summary>
            public static readonly Field CreateUserID = FindByName(__.CreateUserID);

            /// <summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName(__.CreateTime);

            /// <summary>创建地址</summary>
            public static readonly Field CreateIP = FindByName(__.CreateIP);

            /// <summary>更新者</summary>
            public static readonly Field UpdateUserID = FindByName(__.UpdateUserID);

            /// <summary>更新时间</summary>
            public static readonly Field UpdateTime = FindByName(__.UpdateTime);

            /// <summary>更新地址</summary>
            public static readonly Field UpdateIP = FindByName(__.UpdateIP);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
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

    /// <summary>仓库接口</summary>
    public partial interface IRepository
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>拥有者</summary>
        Int32 OwnerID { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>私有</summary>
        Boolean IsPrivate { get; set; }

        /// <summary>匿名读</summary>
        Boolean AllowAnonymousRead { get; set; }

        /// <summary>匿名写</summary>
        Boolean AllowAnonymousWrite { get; set; }

        /// <summary>提交数</summary>
        Int32 Commits { get; set; }

        /// <summary>分支数</summary>
        Int32 Branches { get; set; }

        /// <summary>参与者</summary>
        Int32 Contributors { get; set; }

        /// <summary>文件数</summary>
        Int32 Files { get; set; }

        /// <summary>源码大小</summary>
        Int64 Size { get; set; }

        /// <summary>最后提交</summary>
        DateTime LastCommit { get; set; }

        /// <summary>浏览数</summary>
        Int32 Views { get; set; }

        /// <summary>下载数</summary>
        Int32 Downloads { get; set; }

        /// <summary>最后浏览</summary>
        DateTime LastView { get; set; }

        /// <summary>描述</summary>
        String Description { get; set; }

        /// <summary>创建者</summary>
        Int32 CreateUserID { get; set; }

        /// <summary>创建时间</summary>
        DateTime CreateTime { get; set; }

        /// <summary>创建地址</summary>
        String CreateIP { get; set; }

        /// <summary>更新者</summary>
        Int32 UpdateUserID { get; set; }

        /// <summary>更新时间</summary>
        DateTime UpdateTime { get; set; }

        /// <summary>更新地址</summary>
        String UpdateIP { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}