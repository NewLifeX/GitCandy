﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.GitCandy.Entity
{
    /// <summary>SSH密钥</summary>
    [Serializable]
    [DataObject]
    [Description("SSH密钥")]
    [BindIndex("IU_SshKey_UserID", true, "UserID")]
    [BindIndex("IX_SshKey_Fingerprint", false, "Fingerprint")]
    [BindTable("SshKey", Description = "SSH密钥", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class SshKey : ISshKey
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 10)]
        [BindColumn(1, "ID", "编号", null, "int", 10, 0, false)]
        public virtual Int32 ID
        {
            get { return _ID; }
            set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } }
        }

        private Int32 _UserID;
        /// <summary>用户</summary>
        [DisplayName("用户")]
        [Description("用户")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(2, "UserID", "用户", null, "int", 10, 0, false)]
        public virtual Int32 UserID
        {
            get { return _UserID; }
            set { if (OnPropertyChanging(__.UserID, value)) { _UserID = value; OnPropertyChanged(__.UserID); } }
        }

        private String _KeyType;
        /// <summary>密钥类型</summary>
        [DisplayName("密钥类型")]
        [Description("密钥类型")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(3, "KeyType", "密钥类型", null, "nvarchar(50)", 0, 0, true)]
        public virtual String KeyType
        {
            get { return _KeyType; }
            set { if (OnPropertyChanging(__.KeyType, value)) { _KeyType = value; OnPropertyChanged(__.KeyType); } }
        }

        private String _Fingerprint;
        /// <summary>指纹</summary>
        [DisplayName("指纹")]
        [Description("指纹")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(4, "Fingerprint", "指纹", null, "nvarchar(50)", 0, 0, true)]
        public virtual String Fingerprint
        {
            get { return _Fingerprint; }
            set { if (OnPropertyChanging(__.Fingerprint, value)) { _Fingerprint = value; OnPropertyChanged(__.Fingerprint); } }
        }

        private String _PublicKey;
        /// <summary>公钥</summary>
        [DisplayName("公钥")]
        [Description("公钥")]
        [DataObjectField(false, false, true, 600)]
        [BindColumn(5, "PublicKey", "公钥", null, "nvarchar(600)", 0, 0, true)]
        public virtual String PublicKey
        {
            get { return _PublicKey; }
            set { if (OnPropertyChanging(__.PublicKey, value)) { _PublicKey = value; OnPropertyChanged(__.PublicKey); } }
        }

        private DateTime _ImportData;
        /// <summary>导入数据</summary>
        [DisplayName("导入数据")]
        [Description("导入数据")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(6, "ImportData", "导入数据", null, "datetime", 3, 0, false)]
        public virtual DateTime ImportData
        {
            get { return _ImportData; }
            set { if (OnPropertyChanging(__.ImportData, value)) { _ImportData = value; OnPropertyChanged(__.ImportData); } }
        }

        private DateTime _LastUse;
        /// <summary>最后使用</summary>
        [DisplayName("最后使用")]
        [Description("最后使用")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(7, "LastUse", "最后使用", null, "datetime", 3, 0, false)]
        public virtual DateTime LastUse
        {
            get { return _LastUse; }
            set { if (OnPropertyChanging(__.LastUse, value)) { _LastUse = value; OnPropertyChanged(__.LastUse); } }
        }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(8, "CreateUserID", "创建者", null, "int", 10, 0, false)]
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
        [BindColumn(9, "CreateTime", "创建时间", null, "datetime", 3, 0, false)]
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
        [BindColumn(10, "CreateIP", "创建地址", null, "nvarchar(50)", 0, 0, true)]
        public virtual String CreateIP
        {
            get { return _CreateIP; }
            set { if (OnPropertyChanging(__.CreateIP, value)) { _CreateIP = value; OnPropertyChanged(__.CreateIP); } }
        }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(11, "UpdateUserID", "更新者", null, "int", 10, 0, false)]
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
        [BindColumn(12, "UpdateTime", "更新时间", null, "datetime", 3, 0, false)]
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
        [BindColumn(13, "UpdateIP", "更新地址", null, "nvarchar(50)", 0, 0, true)]
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
                    case __.KeyType : return _KeyType;
                    case __.Fingerprint : return _Fingerprint;
                    case __.PublicKey : return _PublicKey;
                    case __.ImportData : return _ImportData;
                    case __.LastUse : return _LastUse;
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
                    case __.KeyType : _KeyType = Convert.ToString(value); break;
                    case __.Fingerprint : _Fingerprint = Convert.ToString(value); break;
                    case __.PublicKey : _PublicKey = Convert.ToString(value); break;
                    case __.ImportData : _ImportData = Convert.ToDateTime(value); break;
                    case __.LastUse : _LastUse = Convert.ToDateTime(value); break;
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
        /// <summary>取得SSH密钥字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>用户</summary>
            public static readonly Field UserID = FindByName(__.UserID);

            ///<summary>密钥类型</summary>
            public static readonly Field KeyType = FindByName(__.KeyType);

            ///<summary>指纹</summary>
            public static readonly Field Fingerprint = FindByName(__.Fingerprint);

            ///<summary>公钥</summary>
            public static readonly Field PublicKey = FindByName(__.PublicKey);

            ///<summary>导入数据</summary>
            public static readonly Field ImportData = FindByName(__.ImportData);

            ///<summary>最后使用</summary>
            public static readonly Field LastUse = FindByName(__.LastUse);

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

        /// <summary>取得SSH密钥字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>用户</summary>
            public const String UserID = "UserID";

            ///<summary>密钥类型</summary>
            public const String KeyType = "KeyType";

            ///<summary>指纹</summary>
            public const String Fingerprint = "Fingerprint";

            ///<summary>公钥</summary>
            public const String PublicKey = "PublicKey";

            ///<summary>导入数据</summary>
            public const String ImportData = "ImportData";

            ///<summary>最后使用</summary>
            public const String LastUse = "LastUse";

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

    /// <summary>SSH密钥接口</summary>
    public partial interface ISshKey
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>用户</summary>
        Int32 UserID { get; set; }

        /// <summary>密钥类型</summary>
        String KeyType { get; set; }

        /// <summary>指纹</summary>
        String Fingerprint { get; set; }

        /// <summary>公钥</summary>
        String PublicKey { get; set; }

        /// <summary>导入数据</summary>
        DateTime ImportData { get; set; }

        /// <summary>最后使用</summary>
        DateTime LastUse { get; set; }

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