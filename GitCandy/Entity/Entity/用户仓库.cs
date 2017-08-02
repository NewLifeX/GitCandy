﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.GitCandy.Entity
{
    /// <summary>用户仓库</summary>
    [Serializable]
    [DataObject]
    [Description("用户仓库")]
    [BindIndex("IU_UserRepository_UserID_RepositoryID", true, "UserID,RepositoryID")]
    [BindTable("UserRepository", Description = "用户仓库", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class UserRepository : IUserRepository
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 10)]
        [BindColumn("ID", "编号", "int", 10, 0)]
        public virtual Int32 ID
        {
            get { return _ID; }
            set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } }
        }

        private Int32 _UserID;
        /// <summary>用户</summary>
        [DisplayName("用户")]
        [Description("用户")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("UserID", "用户", "int", 10, 0)]
        public virtual Int32 UserID
        {
            get { return _UserID; }
            set { if (OnPropertyChanging(__.UserID, value)) { _UserID = value; OnPropertyChanged(__.UserID); } }
        }

        private Int32 _RepositoryID;
        /// <summary>仓库</summary>
        [DisplayName("仓库")]
        [Description("仓库")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("RepositoryID", "仓库", "int", 10, 0)]
        public virtual Int32 RepositoryID
        {
            get { return _RepositoryID; }
            set { if (OnPropertyChanging(__.RepositoryID, value)) { _RepositoryID = value; OnPropertyChanged(__.RepositoryID); } }
        }

        private Boolean _AllowRead;
        /// <summary>允许读</summary>
        [DisplayName("允许读")]
        [Description("允许读")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("AllowRead", "允许读", "bit", 0, 0)]
        public virtual Boolean AllowRead
        {
            get { return _AllowRead; }
            set { if (OnPropertyChanging(__.AllowRead, value)) { _AllowRead = value; OnPropertyChanged(__.AllowRead); } }
        }

        private Boolean _AllowWrite;
        /// <summary>允许写</summary>
        [DisplayName("允许写")]
        [Description("允许写")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("AllowWrite", "允许写", "bit", 0, 0)]
        public virtual Boolean AllowWrite
        {
            get { return _AllowWrite; }
            set { if (OnPropertyChanging(__.AllowWrite, value)) { _AllowWrite = value; OnPropertyChanged(__.AllowWrite); } }
        }

        private Boolean _IsOwner;
        /// <summary>拥有者</summary>
        [DisplayName("拥有者")]
        [Description("拥有者")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("IsOwner", "拥有者", "bit", 0, 0)]
        public virtual Boolean IsOwner
        {
            get { return _IsOwner; }
            set { if (OnPropertyChanging(__.IsOwner, value)) { _IsOwner = value; OnPropertyChanged(__.IsOwner); } }
        }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("CreateUserID", "创建者", "int", 10, 0)]
        public virtual Int32 CreateUserID
        {
            get { return _CreateUserID; }
            set { if (OnPropertyChanging(__.CreateUserID, value)) { _CreateUserID = value; OnPropertyChanged(__.CreateUserID); } }
        }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("CreateTime", "创建时间", "datetime", 3, 0)]
        public virtual DateTime CreateTime
        {
            get { return _CreateTime; }
            set { if (OnPropertyChanging(__.CreateTime, value)) { _CreateTime = value; OnPropertyChanged(__.CreateTime); } }
        }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "nvarchar(50)", 0, 0)]
        public virtual String CreateIP
        {
            get { return _CreateIP; }
            set { if (OnPropertyChanging(__.CreateIP, value)) { _CreateIP = value; OnPropertyChanged(__.CreateIP); } }
        }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("UpdateUserID", "更新者", "int", 10, 0)]
        public virtual Int32 UpdateUserID
        {
            get { return _UpdateUserID; }
            set { if (OnPropertyChanging(__.UpdateUserID, value)) { _UpdateUserID = value; OnPropertyChanged(__.UpdateUserID); } }
        }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("UpdateTime", "更新时间", "datetime", 3, 0)]
        public virtual DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { if (OnPropertyChanging(__.UpdateTime, value)) { _UpdateTime = value; OnPropertyChanged(__.UpdateTime); } }
        }

        private String _UpdateIP;
        /// <summary>更新地址</summary>
        [DisplayName("更新地址")]
        [Description("更新地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateIP", "更新地址", "nvarchar(50)", 0, 0)]
        public virtual String UpdateIP
        {
            get { return _UpdateIP; }
            set { if (OnPropertyChanging(__.UpdateIP, value)) { _UpdateIP = value; OnPropertyChanged(__.UpdateIP); } }
        }
        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case __.ID : return _ID;
                    case __.UserID : return _UserID;
                    case __.RepositoryID : return _RepositoryID;
                    case __.AllowRead : return _AllowRead;
                    case __.AllowWrite : return _AllowWrite;
                    case __.IsOwner : return _IsOwner;
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
                    case __.UserID : _UserID = Convert.ToInt32(value); break;
                    case __.RepositoryID : _RepositoryID = Convert.ToInt32(value); break;
                    case __.AllowRead : _AllowRead = Convert.ToBoolean(value); break;
                    case __.AllowWrite : _AllowWrite = Convert.ToBoolean(value); break;
                    case __.IsOwner : _IsOwner = Convert.ToBoolean(value); break;
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
        /// <summary>取得用户仓库字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>用户</summary>
            public static readonly Field UserID = FindByName(__.UserID);

            ///<summary>仓库</summary>
            public static readonly Field RepositoryID = FindByName(__.RepositoryID);

            ///<summary>允许读</summary>
            public static readonly Field AllowRead = FindByName(__.AllowRead);

            ///<summary>允许写</summary>
            public static readonly Field AllowWrite = FindByName(__.AllowWrite);

            ///<summary>拥有者</summary>
            public static readonly Field IsOwner = FindByName(__.IsOwner);

            ///<summary>创建者</summary>
            public static readonly Field CreateUserID = FindByName(__.CreateUserID);

            ///<summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName(__.CreateTime);

            ///<summary>创建地址</summary>
            public static readonly Field CreateIP = FindByName(__.CreateIP);

            ///<summary>更新者</summary>
            public static readonly Field UpdateUserID = FindByName(__.UpdateUserID);

            ///<summary>更新时间</summary>
            public static readonly Field UpdateTime = FindByName(__.UpdateTime);

            ///<summary>更新地址</summary>
            public static readonly Field UpdateIP = FindByName(__.UpdateIP);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得用户仓库字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>用户</summary>
            public const String UserID = "UserID";

            ///<summary>仓库</summary>
            public const String RepositoryID = "RepositoryID";

            ///<summary>允许读</summary>
            public const String AllowRead = "AllowRead";

            ///<summary>允许写</summary>
            public const String AllowWrite = "AllowWrite";

            ///<summary>拥有者</summary>
            public const String IsOwner = "IsOwner";

            ///<summary>创建者</summary>
            public const String CreateUserID = "CreateUserID";

            ///<summary>创建时间</summary>
            public const String CreateTime = "CreateTime";

            ///<summary>创建地址</summary>
            public const String CreateIP = "CreateIP";

            ///<summary>更新者</summary>
            public const String UpdateUserID = "UpdateUserID";

            ///<summary>更新时间</summary>
            public const String UpdateTime = "UpdateTime";

            ///<summary>更新地址</summary>
            public const String UpdateIP = "UpdateIP";

        }
        #endregion
    }

    /// <summary>用户仓库接口</summary>
    public partial interface IUserRepository
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>用户</summary>
        Int32 UserID { get; set; }

        /// <summary>仓库</summary>
        Int32 RepositoryID { get; set; }

        /// <summary>允许读</summary>
        Boolean AllowRead { get; set; }

        /// <summary>允许写</summary>
        Boolean AllowWrite { get; set; }

        /// <summary>拥有者</summary>
        Boolean IsOwner { get; set; }

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
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}