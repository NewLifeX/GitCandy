﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [BindTable("User", Description = "用户", ConnName = "GitCandy", DbType = DatabaseType.SqlServer)]
    public partial class User : IUser
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

        private String _Name;
        /// <summary>名称。登录用户名</summary>
        [DisplayName("名称")]
        [Description("名称。登录用户名")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Name", "名称。登录用户名", "nvarchar(50)", 0, 0, Master=true)]
        public virtual String Name
        {
            get { return _Name; }
            set { if (OnPropertyChanging(__.Name, value)) { _Name = value; OnPropertyChanged(__.Name); } }
        }

        private String _Nickname;
        /// <summary>显示名。昵称、中文名等</summary>
        [DisplayName("显示名")]
        [Description("显示名。昵称、中文名等")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Nickname", "显示名。昵称、中文名等", "nvarchar(50)", 0, 0)]
        public virtual String Nickname
        {
            get { return _Nickname; }
            set { if (OnPropertyChanging(__.Nickname, value)) { _Nickname = value; OnPropertyChanged(__.Nickname); } }
        }

        private String _Email;
        /// <summary>邮件</summary>
        [DisplayName("邮件")]
        [Description("邮件")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Email", "邮件", "nvarchar(50)", 0, 0)]
        public virtual String Email
        {
            get { return _Email; }
            set { if (OnPropertyChanging(__.Email, value)) { _Email = value; OnPropertyChanged(__.Email); } }
        }

        private String _Password;
        /// <summary>密码</summary>
        [DisplayName("密码")]
        [Description("密码")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Password", "密码", "nvarchar(50)", 0, 0)]
        public virtual String Password
        {
            get { return _Password; }
            set { if (OnPropertyChanging(__.Password, value)) { _Password = value; OnPropertyChanged(__.Password); } }
        }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("Enable", "启用", "bit", 0, 0)]
        public virtual Boolean Enable
        {
            get { return _Enable; }
            set { if (OnPropertyChanging(__.Enable, value)) { _Enable = value; OnPropertyChanged(__.Enable); } }
        }

        private Boolean _IsTeam;
        /// <summary>团队</summary>
        [DisplayName("团队")]
        [Description("团队")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("IsTeam", "团队", "bit", 0, 0)]
        public virtual Boolean IsTeam
        {
            get { return _IsTeam; }
            set { if (OnPropertyChanging(__.IsTeam, value)) { _IsTeam = value; OnPropertyChanged(__.IsTeam); } }
        }

        private Boolean _IsAdmin;
        /// <summary>管理员</summary>
        [DisplayName("管理员")]
        [Description("管理员")]
        [DataObjectField(false, false, false, 1)]
        [BindColumn("IsAdmin", "管理员", "bit", 0, 0)]
        public virtual Boolean IsAdmin
        {
            get { return _IsAdmin; }
            set { if (OnPropertyChanging(__.IsAdmin, value)) { _IsAdmin = value; OnPropertyChanged(__.IsAdmin); } }
        }

        private DateTime _RegisterTime;
        /// <summary>注册时间</summary>
        [DisplayName("注册时间")]
        [Description("注册时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("RegisterTime", "注册时间", "datetime", 3, 0)]
        public virtual DateTime RegisterTime
        {
            get { return _RegisterTime; }
            set { if (OnPropertyChanging(__.RegisterTime, value)) { _RegisterTime = value; OnPropertyChanged(__.RegisterTime); } }
        }

        private Int32 _Logins;
        /// <summary>登录</summary>
        [DisplayName("登录")]
        [Description("登录")]
        [DataObjectField(false, false, false, 10)]
        [BindColumn("Logins", "登录", "int", 10, 0)]
        public virtual Int32 Logins
        {
            get { return _Logins; }
            set { if (OnPropertyChanging(__.Logins, value)) { _Logins = value; OnPropertyChanged(__.Logins); } }
        }

        private DateTime _LastLogin;
        /// <summary>最后登录</summary>
        [DisplayName("最后登录")]
        [Description("最后登录")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn("LastLogin", "最后登录", "datetime", 3, 0)]
        public virtual DateTime LastLogin
        {
            get { return _LastLogin; }
            set { if (OnPropertyChanging(__.LastLogin, value)) { _LastLogin = value; OnPropertyChanged(__.LastLogin); } }
        }

        private String _LastLoginIP;
        /// <summary>最后登录IP</summary>
        [DisplayName("最后登录IP")]
        [Description("最后登录IP")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("LastLoginIP", "最后登录IP", "nvarchar(50)", 0, 0)]
        public virtual String LastLoginIP
        {
            get { return _LastLoginIP; }
            set { if (OnPropertyChanging(__.LastLoginIP, value)) { _LastLoginIP = value; OnPropertyChanged(__.LastLoginIP); } }
        }

        private String _Description;
        /// <summary>描述</summary>
        [DisplayName("描述")]
        [Description("描述")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn("Description", "描述", "nvarchar(500)", 0, 0)]
        public virtual String Description
        {
            get { return _Description; }
            set { if (OnPropertyChanging(__.Description, value)) { _Description = value; OnPropertyChanged(__.Description); } }
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
                    case __.Name : return _Name;
                    case __.Nickname : return _Nickname;
                    case __.Email : return _Email;
                    case __.Password : return _Password;
                    case __.Enable : return _Enable;
                    case __.IsTeam : return _IsTeam;
                    case __.IsAdmin : return _IsAdmin;
                    case __.RegisterTime : return _RegisterTime;
                    case __.Logins : return _Logins;
                    case __.LastLogin : return _LastLogin;
                    case __.LastLoginIP : return _LastLoginIP;
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
                    case __.Name : _Name = Convert.ToString(value); break;
                    case __.Nickname : _Nickname = Convert.ToString(value); break;
                    case __.Email : _Email = Convert.ToString(value); break;
                    case __.Password : _Password = Convert.ToString(value); break;
                    case __.Enable : _Enable = Convert.ToBoolean(value); break;
                    case __.IsTeam : _IsTeam = Convert.ToBoolean(value); break;
                    case __.IsAdmin : _IsAdmin = Convert.ToBoolean(value); break;
                    case __.RegisterTime : _RegisterTime = Convert.ToDateTime(value); break;
                    case __.Logins : _Logins = Convert.ToInt32(value); break;
                    case __.LastLogin : _LastLogin = Convert.ToDateTime(value); break;
                    case __.LastLoginIP : _LastLoginIP = Convert.ToString(value); break;
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
        /// <summary>取得用户字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>名称。登录用户名</summary>
            public static readonly Field Name = FindByName(__.Name);

            ///<summary>显示名。昵称、中文名等</summary>
            public static readonly Field Nickname = FindByName(__.Nickname);

            ///<summary>邮件</summary>
            public static readonly Field Email = FindByName(__.Email);

            ///<summary>密码</summary>
            public static readonly Field Password = FindByName(__.Password);

            ///<summary>启用</summary>
            public static readonly Field Enable = FindByName(__.Enable);

            ///<summary>团队</summary>
            public static readonly Field IsTeam = FindByName(__.IsTeam);

            ///<summary>管理员</summary>
            public static readonly Field IsAdmin = FindByName(__.IsAdmin);

            ///<summary>注册时间</summary>
            public static readonly Field RegisterTime = FindByName(__.RegisterTime);

            ///<summary>登录</summary>
            public static readonly Field Logins = FindByName(__.Logins);

            ///<summary>最后登录</summary>
            public static readonly Field LastLogin = FindByName(__.LastLogin);

            ///<summary>最后登录IP</summary>
            public static readonly Field LastLoginIP = FindByName(__.LastLoginIP);

            ///<summary>描述</summary>
            public static readonly Field Description = FindByName(__.Description);

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

        /// <summary>取得用户字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>名称。登录用户名</summary>
            public const String Name = "Name";

            ///<summary>显示名。昵称、中文名等</summary>
            public const String Nickname = "Nickname";

            ///<summary>邮件</summary>
            public const String Email = "Email";

            ///<summary>密码</summary>
            public const String Password = "Password";

            ///<summary>启用</summary>
            public const String Enable = "Enable";

            ///<summary>团队</summary>
            public const String IsTeam = "IsTeam";

            ///<summary>管理员</summary>
            public const String IsAdmin = "IsAdmin";

            ///<summary>注册时间</summary>
            public const String RegisterTime = "RegisterTime";

            ///<summary>登录</summary>
            public const String Logins = "Logins";

            ///<summary>最后登录</summary>
            public const String LastLogin = "LastLogin";

            ///<summary>最后登录IP</summary>
            public const String LastLoginIP = "LastLoginIP";

            ///<summary>描述</summary>
            public const String Description = "Description";

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

    /// <summary>用户接口</summary>
    public partial interface IUser
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称。登录用户名</summary>
        String Name { get; set; }

        /// <summary>显示名。昵称、中文名等</summary>
        String Nickname { get; set; }

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

        /// <summary>注册时间</summary>
        DateTime RegisterTime { get; set; }

        /// <summary>登录</summary>
        Int32 Logins { get; set; }

        /// <summary>最后登录</summary>
        DateTime LastLogin { get; set; }

        /// <summary>最后登录IP</summary>
        String LastLoginIP { get; set; }

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
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}