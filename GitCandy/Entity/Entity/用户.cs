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
    /// <summary>用户</summary>
    [Serializable]
    [DataObject]
    [Description("用户")]
    [BindIndex("IU_User_Name", true, "Name")]
    [BindIndex("IX_User_LinkID", false, "LinkID")]
    [BindTable("User", Description = "用户", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class User
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("ID", "编号", "")]
        public Int32 ID { get => _ID; set { if (OnPropertyChanging("ID", value)) { _ID = value; OnPropertyChanged("ID"); } } }

        private String _Name;
        /// <summary>名称。登录用户名</summary>
        [DisplayName("名称")]
        [Description("名称。登录用户名")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称。登录用户名", "", Master = true)]
        public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

        private String _NickName;
        /// <summary>显示名。昵称、中文名等</summary>
        [DisplayName("显示名")]
        [Description("显示名。昵称、中文名等")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("NickName", "显示名。昵称、中文名等", "")]
        public String NickName { get => _NickName; set { if (OnPropertyChanging("NickName", value)) { _NickName = value; OnPropertyChanged("NickName"); } } }

        private String _Email;
        /// <summary>邮件</summary>
        [DisplayName("邮件")]
        [Description("邮件")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Email", "邮件", "")]
        public String Email { get => _Email; set { if (OnPropertyChanging("Email", value)) { _Email = value; OnPropertyChanged("Email"); } } }

        private String _Password;
        /// <summary>密码</summary>
        [DisplayName("密码")]
        [Description("密码")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Password", "密码", "")]
        public String Password { get => _Password; set { if (OnPropertyChanging("Password", value)) { _Password = value; OnPropertyChanged("Password"); } } }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Enable", "启用", "")]
        public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

        private Boolean _IsTeam;
        /// <summary>团队</summary>
        [DisplayName("团队")]
        [Description("团队")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsTeam", "团队", "")]
        public Boolean IsTeam { get => _IsTeam; set { if (OnPropertyChanging("IsTeam", value)) { _IsTeam = value; OnPropertyChanged("IsTeam"); } } }

        private Boolean _IsAdmin;
        /// <summary>管理员</summary>
        [DisplayName("管理员")]
        [Description("管理员")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("IsAdmin", "管理员", "")]
        public Boolean IsAdmin { get => _IsAdmin; set { if (OnPropertyChanging("IsAdmin", value)) { _IsAdmin = value; OnPropertyChanged("IsAdmin"); } } }

        private Boolean _Online;
        /// <summary>在线</summary>
        [DisplayName("在线")]
        [Description("在线")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Online", "在线", "")]
        public Boolean Online { get => _Online; set { if (OnPropertyChanging("Online", value)) { _Online = value; OnPropertyChanged("Online"); } } }

        private DateTime _RegisterTime;
        /// <summary>注册时间</summary>
        [DisplayName("注册时间")]
        [Description("注册时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("RegisterTime", "注册时间", "")]
        public DateTime RegisterTime { get => _RegisterTime; set { if (OnPropertyChanging("RegisterTime", value)) { _RegisterTime = value; OnPropertyChanged("RegisterTime"); } } }

        private String _RegisterIP;
        /// <summary>注册IP</summary>
        [DisplayName("注册IP")]
        [Description("注册IP")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("RegisterIP", "注册IP", "")]
        public String RegisterIP { get => _RegisterIP; set { if (OnPropertyChanging("RegisterIP", value)) { _RegisterIP = value; OnPropertyChanged("RegisterIP"); } } }

        private Int32 _Logins;
        /// <summary>登录</summary>
        [DisplayName("登录")]
        [Description("登录")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Logins", "登录", "")]
        public Int32 Logins { get => _Logins; set { if (OnPropertyChanging("Logins", value)) { _Logins = value; OnPropertyChanged("Logins"); } } }

        private DateTime _LastLogin;
        /// <summary>最后登录</summary>
        [DisplayName("最后登录")]
        [Description("最后登录")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("LastLogin", "最后登录", "")]
        public DateTime LastLogin { get => _LastLogin; set { if (OnPropertyChanging("LastLogin", value)) { _LastLogin = value; OnPropertyChanged("LastLogin"); } } }

        private String _LastLoginIP;
        /// <summary>最后登录IP</summary>
        [DisplayName("最后登录IP")]
        [Description("最后登录IP")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("LastLoginIP", "最后登录IP", "")]
        public String LastLoginIP { get => _LastLoginIP; set { if (OnPropertyChanging("LastLoginIP", value)) { _LastLoginIP = value; OnPropertyChanged("LastLoginIP"); } } }

        private String _Description;
        /// <summary>描述</summary>
        [DisplayName("描述")]
        [Description("描述")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn("Description", "描述", "")]
        public String Description { get => _Description; set { if (OnPropertyChanging("Description", value)) { _Description = value; OnPropertyChanged("Description"); } } }

        private Int32 _LinkID;
        /// <summary>链接。连接到基础用户</summary>
        [DisplayName("链接")]
        [Description("链接。连接到基础用户")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("LinkID", "链接。连接到基础用户", "")]
        public Int32 LinkID { get => _LinkID; set { if (OnPropertyChanging("LinkID", value)) { _LinkID = value; OnPropertyChanged("LinkID"); } } }

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
                    case "Name": return _Name;
                    case "NickName": return _NickName;
                    case "Email": return _Email;
                    case "Password": return _Password;
                    case "Enable": return _Enable;
                    case "IsTeam": return _IsTeam;
                    case "IsAdmin": return _IsAdmin;
                    case "Online": return _Online;
                    case "RegisterTime": return _RegisterTime;
                    case "RegisterIP": return _RegisterIP;
                    case "Logins": return _Logins;
                    case "LastLogin": return _LastLogin;
                    case "LastLoginIP": return _LastLoginIP;
                    case "Description": return _Description;
                    case "LinkID": return _LinkID;
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
                    case "Name": _Name = Convert.ToString(value); break;
                    case "NickName": _NickName = Convert.ToString(value); break;
                    case "Email": _Email = Convert.ToString(value); break;
                    case "Password": _Password = Convert.ToString(value); break;
                    case "Enable": _Enable = value.ToBoolean(); break;
                    case "IsTeam": _IsTeam = value.ToBoolean(); break;
                    case "IsAdmin": _IsAdmin = value.ToBoolean(); break;
                    case "Online": _Online = value.ToBoolean(); break;
                    case "RegisterTime": _RegisterTime = value.ToDateTime(); break;
                    case "RegisterIP": _RegisterIP = Convert.ToString(value); break;
                    case "Logins": _Logins = value.ToInt(); break;
                    case "LastLogin": _LastLogin = value.ToDateTime(); break;
                    case "LastLoginIP": _LastLoginIP = Convert.ToString(value); break;
                    case "Description": _Description = Convert.ToString(value); break;
                    case "LinkID": _LinkID = value.ToInt(); break;
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
        /// <summary>取得用户字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field ID = FindByName("ID");

            /// <summary>名称。登录用户名</summary>
            public static readonly Field Name = FindByName("Name");

            /// <summary>显示名。昵称、中文名等</summary>
            public static readonly Field NickName = FindByName("NickName");

            /// <summary>邮件</summary>
            public static readonly Field Email = FindByName("Email");

            /// <summary>密码</summary>
            public static readonly Field Password = FindByName("Password");

            /// <summary>启用</summary>
            public static readonly Field Enable = FindByName("Enable");

            /// <summary>团队</summary>
            public static readonly Field IsTeam = FindByName("IsTeam");

            /// <summary>管理员</summary>
            public static readonly Field IsAdmin = FindByName("IsAdmin");

            /// <summary>在线</summary>
            public static readonly Field Online = FindByName("Online");

            /// <summary>注册时间</summary>
            public static readonly Field RegisterTime = FindByName("RegisterTime");

            /// <summary>注册IP</summary>
            public static readonly Field RegisterIP = FindByName("RegisterIP");

            /// <summary>登录</summary>
            public static readonly Field Logins = FindByName("Logins");

            /// <summary>最后登录</summary>
            public static readonly Field LastLogin = FindByName("LastLogin");

            /// <summary>最后登录IP</summary>
            public static readonly Field LastLoginIP = FindByName("LastLoginIP");

            /// <summary>描述</summary>
            public static readonly Field Description = FindByName("Description");

            /// <summary>链接。连接到基础用户</summary>
            public static readonly Field LinkID = FindByName("LinkID");

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
}