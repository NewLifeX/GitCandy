using System;
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

        public TService GetService<TService>() => throw new NotImplementedException();

        public Object GetService(Type serviceType) => throw new NotImplementedException();

        public IManageUser Login(String name, String password, Boolean rememberme = false) => throw new NotImplementedException();

        public void Logout() => throw new NotImplementedException();

        public IManageUser Register(String name, String password, Int32 roleid, Boolean enable = false) => throw new NotImplementedException();
    }
}