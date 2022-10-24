using NewLife;
using NewLife.Cube.Entity;
using NewLife.Web;
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
            var md5 = password.MD5();

            // 基础用户表找到该用户
            var provider = ManageProvider.Provider;
            var u = provider.FindByID(user.LinkID);
            if (u != null && u.Enable)
            {
                // 以用户令牌登录，替代密码，更安全
                //var tokens = UserToken.FindAllByUserID(user.LinkID);
                var pager = new Pager { PageSize = 100 };
                var tokens = UserToken.Search(null, user.LinkID, true, DateTime.MinValue, DateTime.MinValue, pager);
                foreach (var token in tokens)
                {
                    if (token.Enable && (token.Expire.Year < 1000 || token.Expire > DateTime.Now) && token.Token.EqualIgnoreCase(password, md5))
                    {
                        user.Login(ip);

                        return user;
                    }
                }

                // 基础用户表中验证用户密码
                u = ManageProvider.Provider.Login(u.Name, password, false);
                if (u != null)
                {
                    user.Login(ip);

                    return user;
                }
            }
            // 继续使用原来的密码验证
            else
            {
                if (user.Password == md5)
                {
                    user.Login(ip);

                    return user;
                }
            }
        }

        return null;
    }
}