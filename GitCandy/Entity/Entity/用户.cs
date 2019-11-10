using System;
using System.Collections.Generic;
using System.ComponentModel;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.GitCandy.Entity
{
    /// <summary>用户</summary>
    [Serializable]
    [DataObject]
    [Description("用户")]
    [BindIndex("IU_User_Name", true, "Name")]
    [BindIndex("IX_User_LinkID", false, "LinkID")]
    [BindTable("User", Description = "用户", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class User : IUser
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("ID", "编号", "int")]
        public Int32 ID { get { return _ID; } set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } } }

        private String _Name;
        /// <summary>名称。登录用户名</summary>
        [DisplayName("名称")]
        [Description("名称。登录用户名")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称。登录用户名", "nvarchar(50)", Master = true)]
        public String Name { get { return _Name; } set { if (OnPropertyChanging(__.Name, value)) { _Name = value; OnPropertyChanged(__.Name); } } }

        private String _NickName;
        /// <summary>显示名。昵称、中文名等</summary>
        [DisplayName("显示名")]
        [Description("显示名。昵称、中文名等")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("NickName", "显示名。昵称、中文名等", "nvarchar(50)")]
        public String NickName { get { return _NickName; } set { if (OnPropertyChanging(__.NickName, value)) { _NickName = value; OnPropertyChanged(__.NickName); } } }

        private String _Email;
        /// <summary>邮件</summary>
        [DisplayName("邮件")]
        [Description("邮件")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Email", "邮件", "nvarchar(50)")]
        public String Email { get { return _Email; } set { if (OnPropertyChanging(__.Email, value)) { _Email = value; OnPropertyChanged(__.Email); } } }

        private String _Password;
        /// <summary>密码</summary>
        [DisplayName("密码")]
        [Description("密码")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Password", "密码", "nvarchar(50)")]
        public String Password { get { return _Password; } set { if (OnPropertyChanging(__.Password, value)) { _Password = value; OnPropertyChanged(__.Password); } } }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Enable", "启用", "bit")]
        public Boolean Enable { get { return _Enable; } set { if (OnPropertyChanging(__.Enable, value)) { _Enable = value; OnPropertyChanged(__.Enable); } } }

        private Boolean _IsTeam;
        /// <summary>团队</summary>
        [DisplayName("团队")]
        [Description("团队")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsTeam", "团队", "bit")]
        public Boolean IsTeam { get { return _IsTeam; } set { if (OnPropertyChanging(__.IsTeam, value)) { _IsTeam = value; OnPropertyChanged(__.IsTeam); } } }

        private Boolean _IsAdmin;
        /// <summary>管理员</summary>
        [DisplayName("管理员")]
        [Description("管理员")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsAdmin", "管理员", "bit")]
        public Boolean IsAdmin { get { return _IsAdmin; } set { if (OnPropertyChanging(__.IsAdmin, value)) { _IsAdmin = value; OnPropertyChanged(__.IsAdmin); } } }

        private Boolean _Online;
        /// <summary>在线</summary>
        [DisplayName("在线")]
        [Description("在线")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Online", "在线", "bit")]
        public Boolean Online { get { return _Online; } set { if (OnPropertyChanging(__.Online, value)) { _Online = value; OnPropertyChanged(__.Online); } } }

        private DateTime _RegisterTime;
        /// <summary>注册时间</summary>
        [DisplayName("注册时间")]
        [Description("注册时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("RegisterTime", "注册时间", "datetime")]
        public DateTime RegisterTime { get { return _RegisterTime; } set { if (OnPropertyChanging(__.RegisterTime, value)) { _RegisterTime = value; OnPropertyChanged(__.RegisterTime); } } }

        private String _RegisterIP;
        /// <summary>注册IP</summary>
        [DisplayName("注册IP")]
        [Description("注册IP")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("RegisterIP", "注册IP", "nvarchar(50)")]
        public String RegisterIP { get { return _RegisterIP; } set { if (OnPropertyChanging(__.RegisterIP, value)) { _RegisterIP = value; OnPropertyChanged(__.RegisterIP); } } }

        private Int32 _Logins;
        /// <summary>登录</summary>
        [DisplayName("登录")]
        [Description("登录")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Logins", "登录", "int")]
        public Int32 Logins { get { return _Logins; } set { if (OnPropertyChanging(__.Logins, value)) { _Logins = value; OnPropertyChanged(__.Logins); } } }

        private DateTime _LastLogin;
        /// <summary>最后登录</summary>
        [DisplayName("最后登录")]
        [Description("最后登录")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastLogin", "最后登录", "datetime")]
        public DateTime LastLogin { get { return _LastLogin; } set { if (OnPropertyChanging(__.LastLogin, value)) { _LastLogin = value; OnPropertyChanged(__.LastLogin); } } }

        private String _LastLoginIP;
        /// <summary>最后登录IP</summary>
        [DisplayName("最后登录IP")]
        [Description("最后登录IP")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("LastLoginIP", "最后登录IP", "nvarchar(50)")]
        public String LastLoginIP { get { return _LastLoginIP; } set { if (OnPropertyChanging(__.LastLoginIP, value)) { _LastLoginIP = value; OnPropertyChanged(__.LastLoginIP); } } }

        private String _Description;
        /// <summary>描述</summary>
        [DisplayName("描述")]
        [Description("描述")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn("Description", "描述", "nvarchar(500)")]
        public String Description { get { return _Description; } set { if (OnPropertyChanging(__.Description, value)) { _Description = value; OnPropertyChanged(__.Description); } } }

        private Int32 _LinkID;
        /// <summary>链接。连接到基础用户</summary>
        [DisplayName("链接")]
        [Description("链接。连接到基础用户")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("LinkID", "链接。连接到基础用户", "int")]
        public Int32 LinkID { get { return _LinkID; } set { if (OnPropertyChanging(__.LinkID, value)) { _LinkID = value; OnPropertyChanged(__.LinkID); } } }

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
                    case __.Name : return _Name;
                    case __.NickName : return _NickName;
                    case __.Email : return _Email;
                    case __.Password : return _Password;
                    case __.Enable : return _Enable;
                    case __.IsTeam : return _IsTeam;
                    case __.IsAdmin : return _IsAdmin;
                    case __.Online : return _Online;
                    case __.RegisterTime : return _RegisterTime;
                    case __.RegisterIP : return _RegisterIP;
                    case __.Logins : return _Logins;
                    case __.LastLogin : return _LastLogin;
                    case __.LastLoginIP : return _LastLoginIP;
                    case __.Description : return _Description;
                    case __.LinkID : return _LinkID;
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
                    case __.Name : _Name = Convert.ToString(value); break;
                    case __.NickName : _NickName = Convert.ToString(value); break;
                    case __.Email : _Email = Convert.ToString(value); break;
                    case __.Password : _Password = Convert.ToString(value); break;
                    case __.Enable : _Enable = Convert.ToBoolean(value); break;
                    case __.IsTeam : _IsTeam = Convert.ToBoolean(value); break;
                    case __.IsAdmin : _IsAdmin = Convert.ToBoolean(value); break;
                    case __.Online : _Online = Convert.ToBoolean(value); break;
                    case __.RegisterTime : _RegisterTime = Convert.ToDateTime(value); break;
                    case __.RegisterIP : _RegisterIP = Convert.ToString(value); break;
                    case __.Logins : _Logins = Convert.ToInt32(value); break;
                    case __.LastLogin : _LastLogin = Convert.ToDateTime(value); break;
                    case __.LastLoginIP : _LastLoginIP = Convert.ToString(value); break;
                    case __.Description : _Description = Convert.ToString(value); break;
                    case __.LinkID : _LinkID = Convert.ToInt32(value); break;
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
        /// <summary>取得用户字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            /// <summary>名称。登录用户名</summary>
            public static readonly Field Name = FindByName(__.Name);

            /// <summary>显示名。昵称、中文名等</summary>
            public static readonly Field NickName = FindByName(__.NickName);

            /// <summary>邮件</summary>
            public static readonly Field Email = FindByName(__.Email);

            /// <summary>密码</summary>
            public static readonly Field Password = FindByName(__.Password);

            /// <summary>启用</summary>
            public static readonly Field Enable = FindByName(__.Enable);

            /// <summary>团队</summary>
            public static readonly Field IsTeam = FindByName(__.IsTeam);

            /// <summary>管理员</summary>
            public static readonly Field IsAdmin = FindByName(__.IsAdmin);

            /// <summary>在线</summary>
            public static readonly Field Online = FindByName(__.Online);

            /// <summary>注册时间</summary>
            public static readonly Field RegisterTime = FindByName(__.RegisterTime);

            /// <summary>注册IP</summary>
            public static readonly Field RegisterIP = FindByName(__.RegisterIP);

            /// <summary>登录</summary>
            public static readonly Field Logins = FindByName(__.Logins);

            /// <summary>最后登录</summary>
            public static readonly Field LastLogin = FindByName(__.LastLogin);

            /// <summary>最后登录IP</summary>
            public static readonly Field LastLoginIP = FindByName(__.LastLoginIP);

            /// <summary>描述</summary>
            public static readonly Field Description = FindByName(__.Description);

            /// <summary>链接。连接到基础用户</summary>
            public static readonly Field LinkID = FindByName(__.LinkID);

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

        /// <summary>取得用户字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>编号</summary>
            public const String ID = "ID";

            /// <summary>名称。登录用户名</summary>
            public const String Name = "Name";

            /// <summary>显示名。昵称、中文名等</summary>
            public const String NickName = "NickName";

            /// <summary>邮件</summary>
            public const String Email = "Email";

            /// <summary>密码</summary>
            public const String Password = "Password";

            /// <summary>启用</summary>
            public const String Enable = "Enable";

            /// <summary>团队</summary>
            public const String IsTeam = "IsTeam";

            /// <summary>管理员</summary>
            public const String IsAdmin = "IsAdmin";

            /// <summary>在线</summary>
            public const String Online = "Online";

            /// <summary>注册时间</summary>
            public const String RegisterTime = "RegisterTime";

            /// <summary>注册IP</summary>
            public const String RegisterIP = "RegisterIP";

            /// <summary>登录</summary>
            public const String Logins = "Logins";

            /// <summary>最后登录</summary>
            public const String LastLogin = "LastLogin";

            /// <summary>最后登录IP</summary>
            public const String LastLoginIP = "LastLoginIP";

            /// <summary>描述</summary>
            public const String Description = "Description";

            /// <summary>链接。连接到基础用户</summary>
            public const String LinkID = "LinkID";

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

    /// <summary>用户接口</summary>
    public partial interface IUser
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称。登录用户名</summary>
        String Name { get; set; }

        /// <summary>显示名。昵称、中文名等</summary>
        String NickName { get; set; }

        /// <summary>邮件</summary>
        String Email { get; set; }

        /// <summary>密码</summary>
        String Password { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>团队</summary>
        Boolean IsTeam { get; set; }

        /// <summary>管理员</summary>
        Boolean IsAdmin { get; set; }

        /// <summary>在线</summary>
        Boolean Online { get; set; }

        /// <summary>注册时间</summary>
        DateTime RegisterTime { get; set; }

        /// <summary>注册IP</summary>
        String RegisterIP { get; set; }

        /// <summary>登录</summary>
        Int32 Logins { get; set; }

        /// <summary>最后登录</summary>
        DateTime LastLogin { get; set; }

        /// <summary>最后登录IP</summary>
        String LastLoginIP { get; set; }

        /// <summary>描述</summary>
        String Description { get; set; }

        /// <summary>链接。连接到基础用户</summary>
        Int32 LinkID { get; set; }

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