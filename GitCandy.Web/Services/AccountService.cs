using NewLife.GitCandy.Entity;
using XCode.Membership;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Services;

public class AccountService
{
    public UserX Login(String username, String password, String ip)
    {
        var user = UserX.Check(username);
        if (user != null)
        {
            // 基础用户表找到该用户
            var provider = ManageProvider.Provider;
            var u = provider.FindByID(user.LinkID);
            if (u != null && u.Enable)
            {
                // 基础用户表中验证用户密码
                u = ManageProvider.Provider.Login(u.Name, password, false);
                if (u != null)
                {
                    user.Login(ip);

                    return user;
                }
            }
        }

        return null;
    }
}