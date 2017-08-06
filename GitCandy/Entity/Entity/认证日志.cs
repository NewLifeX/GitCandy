using System;
using System.Collections.Generic;
using System.ComponentModel;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.GitCandy.Entity
{
    /// <summary>认证日志</summary>
    [Serializable]
    [DataObject]
    [Description("认证日志")]
    [BindIndex("IU_AuthorizationLog_AuthCode", true, "AuthCode")]
    [BindIndex("IX_AuthorizationLog_UserID", false, "UserID")]
    [BindTable("AuthorizationLog", Description = "认证日志", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class AuthorizationLog : IAuthorizationLog
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 10)]
        [BindColumn("ID", "编号", "int", 10, 0)]
        public Int32 ID { get { return _ID; } set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } } }

        private String _AuthCode;
        /// <summary>认证码</summary>
        [DisplayName("认证码")]
        [Description("认证码")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("AuthCode", "认证码", "nvarchar(50)", 0, 0, Master = true)]
        public String AuthCode { get { return _AuthCode; } set { if (OnPropertyChanging(__.AuthCode, value)) { _AuthCode = value; OnPropertyChanged(__.AuthCode); } } }

        private Int32 _UserID;
        /// <summary>用户</summary>
        [DisplayName("用户")]
        [Description("用户")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("UserID", "用户", "int", 10, 0)]
        public Int32 UserID { get { return _UserID; } set { if (OnPropertyChanging(__.UserID, value)) { _UserID = value; OnPropertyChanged(__.UserID); } } }

        private DateTime _IssueDate;
        /// <summary>发生时间</summary>
        [DisplayName("发生时间")]
        [Description("发生时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("IssueDate", "发生时间", "datetime", 3, 0)]
        public DateTime IssueDate { get { return _IssueDate; } set { if (OnPropertyChanging(__.IssueDate, value)) { _IssueDate = value; OnPropertyChanged(__.IssueDate); } } }

        private DateTime _Expires;
        /// <summary>过期时间</summary>
        [DisplayName("过期时间")]
        [Description("过期时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("Expires", "过期时间", "datetime", 3, 0)]
        public DateTime Expires { get { return _Expires; } set { if (OnPropertyChanging(__.Expires, value)) { _Expires = value; OnPropertyChanged(__.Expires); } } }

        private String _IssueIp;
        /// <summary>发生地址</summary>
        [DisplayName("发生地址")]
        [Description("发生地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("IssueIp", "发生地址", "nvarchar(50)", 0, 0)]
        public String IssueIp { get { return _IssueIp; } set { if (OnPropertyChanging(__.IssueIp, value)) { _IssueIp = value; OnPropertyChanged(__.IssueIp); } } }

        private String _LastIp;
        /// <summary>最后地址</summary>
        [DisplayName("最后地址")]
        [Description("最后地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("LastIp", "最后地址", "nvarchar(50)", 0, 0)]
        public String LastIp { get { return _LastIp; } set { if (OnPropertyChanging(__.LastIp, value)) { _LastIp = value; OnPropertyChanged(__.LastIp); } } }

        private Boolean _IsValid;
        /// <summary>有效</summary>
        [DisplayName("有效")]
        [Description("有效")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("IsValid", "有效", "bit", 0, 0)]
        public Boolean IsValid { get { return _IsValid; } set { if (OnPropertyChanging(__.IsValid, value)) { _IsValid = value; OnPropertyChanged(__.IsValid); } } }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("CreateUserID", "创建者", "int", 10, 0)]
        public Int32 CreateUserID { get { return _CreateUserID; } set { if (OnPropertyChanging(__.CreateUserID, value)) { _CreateUserID = value; OnPropertyChanged(__.CreateUserID); } } }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("CreateTime", "创建时间", "datetime", 3, 0)]
        public DateTime CreateTime { get { return _CreateTime; } set { if (OnPropertyChanging(__.CreateTime, value)) { _CreateTime = value; OnPropertyChanged(__.CreateTime); } } }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "nvarchar(50)", 0, 0)]
        public String CreateIP { get { return _CreateIP; } set { if (OnPropertyChanging(__.CreateIP, value)) { _CreateIP = value; OnPropertyChanged(__.CreateIP); } } }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("UpdateUserID", "更新者", "int", 10, 0)]
        public Int32 UpdateUserID { get { return _UpdateUserID; } set { if (OnPropertyChanging(__.UpdateUserID, value)) { _UpdateUserID = value; OnPropertyChanged(__.UpdateUserID); } } }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("UpdateTime", "更新时间", "datetime", 3, 0)]
        public DateTime UpdateTime { get { return _UpdateTime; } set { if (OnPropertyChanging(__.UpdateTime, value)) { _UpdateTime = value; OnPropertyChanged(__.UpdateTime); } } }

        private String _UpdateIP;
        /// <summary>更新地址</summary>
        [DisplayName("更新地址")]
        [Description("更新地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateIP", "更新地址", "nvarchar(50)", 0, 0)]
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
                    case __.AuthCode : return _AuthCode;
                    case __.UserID : return _UserID;
                    case __.IssueDate : return _IssueDate;
                    case __.Expires : return _Expires;
                    case __.IssueIp : return _IssueIp;
                    case __.LastIp : return _LastIp;
                    case __.IsValid : return _IsValid;
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
                    case __.AuthCode : _AuthCode = Convert.ToString(value); break;
                    case __.UserID : _UserID = Convert.ToInt32(value); break;
                    case __.IssueDate : _IssueDate = Convert.ToDateTime(value); break;
                    case __.Expires : _Expires = Convert.ToDateTime(value); break;
                    case __.IssueIp : _IssueIp = Convert.ToString(value); break;
                    case __.LastIp : _LastIp = Convert.ToString(value); break;
                    case __.IsValid : _IsValid = Convert.ToBoolean(value); break;
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
        /// <summary>取得认证日志字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            /// <summary>认证码</summary>
            public static readonly Field AuthCode = FindByName(__.AuthCode);

            /// <summary>用户</summary>
            public static readonly Field UserID = FindByName(__.UserID);

            /// <summary>发生时间</summary>
            public static readonly Field IssueDate = FindByName(__.IssueDate);

            /// <summary>过期时间</summary>
            public static readonly Field Expires = FindByName(__.Expires);

            /// <summary>发生地址</summary>
            public static readonly Field IssueIp = FindByName(__.IssueIp);

            /// <summary>最后地址</summary>
            public static readonly Field LastIp = FindByName(__.LastIp);

            /// <summary>有效</summary>
            public static readonly Field IsValid = FindByName(__.IsValid);

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

        /// <summary>取得认证日志字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>编号</summary>
            public const String ID = "ID";

            /// <summary>认证码</summary>
            public const String AuthCode = "AuthCode";

            /// <summary>用户</summary>
            public const String UserID = "UserID";

            /// <summary>发生时间</summary>
            public const String IssueDate = "IssueDate";

            /// <summary>过期时间</summary>
            public const String Expires = "Expires";

            /// <summary>发生地址</summary>
            public const String IssueIp = "IssueIp";

            /// <summary>最后地址</summary>
            public const String LastIp = "LastIp";

            /// <summary>有效</summary>
            public const String IsValid = "IsValid";

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

    /// <summary>认证日志接口</summary>
    public partial interface IAuthorizationLog
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>认证码</summary>
        String AuthCode { get; set; }

        /// <summary>用户</summary>
        Int32 UserID { get; set; }

        /// <summary>发生时间</summary>
        DateTime IssueDate { get; set; }

        /// <summary>过期时间</summary>
        DateTime Expires { get; set; }

        /// <summary>发生地址</summary>
        String IssueIp { get; set; }

        /// <summary>最后地址</summary>
        String LastIp { get; set; }

        /// <summary>有效</summary>
        Boolean IsValid { get; set; }

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