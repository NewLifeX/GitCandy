using NewLife;
using NewLife.Cube.Entity;
using NewLife.Log;
using NewLife.Web;
using XCode.Membership;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Services;

/// <summary>账号服务。处理用户登录认证，支持令牌Token和密码两种验证方式</summary>
public class AccountService
{
    private readonly ITracer _tracer;

    /// <summary>实例化账号服务</summary>
    /// <param name="tracer">追踪器</param>
    public AccountService(ITracer tracer) => _tracer = tracer;

    /// <summary>用户登录。先尝试令牌Token验证，再尝试密码验证，最后回退到旧版密码</summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码或令牌</param>
    /// <param name="ip">客户端IP</param>
    /// <returns>登录成功返回用户实体，失败返回null</returns>
    public UserX Login(String username, String password, String ip)
    {
        using var span = _tracer?.NewSpan("AccountLogin", new { username, ip });

        var user = UserX.Check(username);
        if (user != null && user.Enable)
        {
            var md5 = password.MD5();

            // 基础用户表找到该用户
            var provider = ManageProvider.Provider;
            var u = provider.FindByID(user.LinkID);
            if (u != null && u.Enable)
            {
                {
                    using var span2 = _tracer?.NewSpan("AccountLogin-Token", new { u.Name, ip });
                    try
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
                    }
                    catch (Exception ex)
                    {
                        span?.SetError(ex, null);
                        throw;
                    }
                }

                {
                    using var span2 = _tracer?.NewSpan("AccountLogin-Password", new { u.Name, ip });
                    try
                    {
                        // 基础用户表中验证用户密码
                        u = ManageProvider.Provider.Login(u.Name, password, false);
                        if (u != null)
                        {
                            user.Login(ip);

                            return user;
                        }
                    }
                    catch (Exception ex)
                    {
                        span?.SetError(ex, null);
                        throw;
                    }
                }
            }
            // 继续使用原来的密码验证
            else
            {
                if (user.Password == md5)
                {
                    using var span2 = _tracer?.NewSpan("AccountLogin-Old", user.Name);
                    user.Login(ip);

                    return user;
                }
            }
        }

        return null;
    }
}