using System;
using System.Web;
using System.Web.SessionState;
using NewLife.GitCandy.Entity;
using NewLife.Model;
using XCode.Membership;

namespace GitCandy.Entity
{
    /// <summary>糖果提供者</summary>
    public class CandyManageProvider : IManageProvider
    {
        public static CandyManageProvider Provider { get; set; } = new CandyManageProvider();

        public IManageUser Current { get => User.Current; set => User.Current = (User)value; }

        public IManageUser FindByID(Object userid) => User.FindByID(userid.ToInt());

        public IManageUser FindByName(String name) => User.FindByName(name);

        /// <summary>保存于Session的凭证</summary>
        public String SessionKey { get; set; } = "Candy";

        public IManageUser GetCurrent(IServiceProvider context)
        {
            if (context == null) context = HttpContext.Current;
            var ss = context.GetService<HttpSessionState>();
            if (ss == null) return null;

            // 从Session中获取
            return ss[SessionKey] as IManageUser;
        }

        public void SetCurrent(IManageUser user, IServiceProvider context)
        {
            if (context == null) context = HttpContext.Current;
            var ss = context.GetService<HttpSessionState>();
            if (ss == null) return;

            var key = SessionKey;
            // 特殊处理注销
            if (user == null)
            {
                // 修改Session
                ss.Remove(key);

                if (ss[key] is IAuthUser au)
                {
                    au.Online = false;
                    au.Save();
                }
            }
            else
            {
                // 修改Session
                ss[key] = user;
            }
        }

        public TService GetService<TService>() => throw new NotImplementedException();

        public Object GetService(Type serviceType) => throw new NotImplementedException();

        public IManageUser Login(String name, String password, Boolean rememberme = false) => throw new NotImplementedException();

        public void Logout() => throw new NotImplementedException();

        public IManageUser Register(String name, String password, Int32 roleid, Boolean enable = false) => throw new NotImplementedException();
    }
}