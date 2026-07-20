using GitCandy.Base;
using GitCandy.Models;
using NewLife;
using NewLife.Data;
using NewLife.GitCandy.Entity;
using XCode;

namespace GitCandy.Data;

/// <summary>成员服务。提供用户/团队数据查询、团队管理、成员管理等功能</summary>
public class MembershipService
{
    #region Account part
    /// <summary>获取用户模型。包含团队和仓库信息</summary>
    /// <param name="name">用户名</param>
    /// <param name="withMembers">是否包含成员</param>
    /// <param name="viewUser">查看者用户名</param>
    /// <returns></returns>
    public UserModel GetUserModel(String name, Boolean withMembers = false, String viewUser = null)
    {
        var user = User.FindByName(name);
        if (user == null) return null;

        var model = new UserModel
        {
            Name = user.Name,
            Nickname = user.NickName,
            Email = user.Email,
            Description = user.Description,
            IsAdmin = user.IsAdmin,
        };
        if (withMembers)
        {
            model.Teams = user.Teams.ToDictionary(e => e.Team.Name, e => e.Team.NickName);
            var rs = user.Repositories.Where(e => e.Repository != null && e.Repository.Enable).Select(e => e.Repository);
            if (!viewUser.IsNullOrEmpty())
            {
                var vu = User.FindByName(viewUser);
                rs = rs.Where(e => e.CanViewFor(vu));
            }
            model.Repositories = rs.Select(e => e.Owner.Name + "@" + e.Name).ToArray();
        }
        return model;
    }

    //public User Login(String id, String password)
    //{
    //    var user = User.FindByName(id) ?? User.FindByEmail(id);
    //    if (user != null && user.Login(password)) return user;

    //    return null;
    //}

    //public void SetPassword(String name, String newPassword)
    //{
    //    var user = User.FindByName(name);
    //    if (user != null)
    //    {
    //        user.Password = newPassword.MD5();

    //        var auths = AuthorizationLog.FindAllByUserID(user.ID);
    //        foreach (var auth in auths)
    //        {
    //            auth.IsValid = false;
    //        }
    //        user.Save();
    //        auths.Save();
    //    }
    //}

    //public Boolean UpdateUser(UserModel model)
    //{
    //    var user = User.FindByName(model.Name);
    //    if (user != null)
    //    {
    //        user.NickName = model.Nickname;
    //        user.Email = model.Email;
    //        user.Description = model.Description;
    //        user.IsAdmin = model.IsAdmin;

    //        user.Save();
    //        return true;
    //    }
    //    return false;
    //}

    //public AuthorizationLog CreateAuthorization(Int64 userID, DateTime expires, String ip)
    //{
    //    var auth = new AuthorizationLog
    //    {
    //        AuthCode = Guid.NewGuid().ToString(),
    //        UserID = (Int32)userID,
    //        IssueDate = DateTime.Now,
    //        Expires = expires,
    //        IssueIp = ip,
    //        LastIp = ip,
    //        IsValid = true,
    //    };
    //    auth.Save();
    //    return auth;
    //}

    //public Token GetToken(Guid authCode)
    //{
    //    var auth = AuthorizationLog.FindByAuthCode(authCode + "");
    //    if (auth == null) return null;

    //    var user = auth.User;

    //    return new Token(auth.AuthCode, auth.UserID, user.Name, user.NickName, user.IsAdmin, auth.Expires)
    //    {
    //        LastIp = auth.LastIp
    //    };
    //}

    //public void UpdateAuthorization(Guid authCode, DateTime expires, String lastIp)
    //{
    //    var auth = AuthorizationLog.FindByAuthCode(authCode + "");
    //    if (auth != null)
    //    {
    //        auth.Expires = expires;
    //        auth.LastIp = lastIp;
    //        auth.Save();
    //    }
    //}

    //public void SetAuthorizationAsInvalid(Guid authCode)
    //{
    //    var auth = AuthorizationLog.FindByAuthCode(authCode + "");
    //    if (auth != null)
    //    {
    //        auth.IsValid = false;
    //        auth.Save();
    //    }
    //}

    //public void DeleteUser(String name)
    //{
    //    var user = User.FindByName(name);
    //    if (user != null) user.Delete();
    //}

    /// <summary>获取用户列表。分页搜索</summary>
    /// <param name="keyword">关键词</param>
    /// <param name="page">页码</param>
    /// <param name="pagesize">每页大小</param>
    /// <returns></returns>
    public UserListModel GetUserList(String keyword, Int32 page, Int32 pagesize = 20)
    {
        var p = new PageParameter
        {
            PageIndex = page,
            PageSize = pagesize,
            RetrieveTotalCount = true,
        };
        var list = User.SearchByName(keyword, p);

        return new UserListModel
        {
            Users = list.ToList().Select(e => new UserModel
            {
                Name = e.Name,
                Nickname = e.NickName,
                Email = e.Email,
                Description = e.Description,
                IsAdmin = e.IsAdmin,
            }).ToArray(),
            CurrentPage = page,
            ItemCount = (Int32)p.TotalCount
        };
    }
    #endregion

    #region Team part
    /// <summary>更新团队信息</summary>
    /// <param name="model">团队模型</param>
    /// <returns></returns>
    public Boolean UpdateTeam(TeamModel model)
    {
        var team = User.FindByName(model.Name);
        if (team == null) return false;

        team.NickName = model.Nickname;
        team.Description = model.Description;
        team.Save();

        return true;
    }

    /// <summary>获取团队模型。包含成员列表</summary>
    /// <param name="name">团队名</param>
    /// <param name="withMembers">是否包含成员</param>
    /// <param name="viewUser">查看者用户名</param>
    /// <returns></returns>
    public TeamModel GetTeamModel(String name, Boolean withMembers = false, String viewUser = null)
    {
        var team = User.FindByName(name);
        if (team == null) return null;

        var model = new TeamModel
        {
            Name = team.Name,
            Nickname = team.NickName,
            Description = team.Description,
        };
        if (withMembers)
        {
            var list = UserTeam.FindAllByTeamID(team.ID).ToList().Where(e => e.User != null).ToList();
            model.MembersRole = list
                .Select(s => new TeamModel.UserRole
                {
                    Name = s.User.Name,
                    NickName = s.User.NickName,
                    IsAdministrator = s.IsAdmin
                })
                .OrderBy(s => s.Name, new StringLogicalComparer())
                .ToArray();
            model.Members = list.ToDictionary(e => e.User.Name, e => e.User.NickName);

            var rs = team.Repositories.Where(e => e.Repository.Enable).ToList();
            if (!viewUser.IsNullOrEmpty())
            {
                var vu = User.FindByName(viewUser);
                rs = rs.Where(e => e.Repository.CanViewFor(vu)).ToList();
            }
            model.RepositoriesRole = rs.Select(s => new TeamModel.RepositoryRole
            {
                Name = s.RepositoryName,
                AllowRead = s.AllowRead,
                AllowWrite = s.AllowWrite,
            })
                .OrderBy(s => s.Name, new StringLogicalComparer())
                .ToArray();
            model.Repositories = rs
                .Select(s => s.Repository.Owner.Name + "@" + s.Repository.Name)
                .ToArray();
        }
        return model;
    }

    /// <summary>添加团队成员</summary>
    /// <param name="teamname">团队名</param>
    /// <param name="username">用户名</param>
    /// <returns></returns>
    public Boolean TeamAddUser(String teamname, String username)
    {
        var team = User.FindByName(teamname);
        if (team == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
        if (tu == null)
        {
            tu = new UserTeam
            {
                UserID = user.ID,
                TeamID = team.ID
            };
            tu.Save();
        }

        return true;
    }

    /// <summary>移除团队成员</summary>
    /// <param name="teamname">团队名</param>
    /// <param name="username">用户名</param>
    /// <returns></returns>
    public Boolean TeamRemoveUser(String teamname, String username)
    {
        var team = User.FindByName(teamname);
        if (team == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
        if (tu == null) return false;

        tu.Delete();

        return true;
    }

    /// <summary>设置团队管理员</summary>
    /// <param name="teamname">团队名</param>
    /// <param name="username">用户名</param>
    /// <param name="isAdmin">是否管理员</param>
    /// <returns></returns>
    public Boolean TeamUserSetAdministrator(String teamname, String username, Boolean isAdmin)
    {
        var team = User.FindByName(teamname);
        if (team == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
        if (tu == null) return false;

        tu.IsAdmin = true;
        tu.Save();

        return true;
    }

    /// <summary>搜索团队（AJAX）</summary>
    /// <param name="query">搜索关键词</param>
    /// <returns></returns>
    public String[] SearchTeam(String query)
    {
        var p = new PageParameter();
        var list = User.SearchTeam(query, p);
        return list.ToList().Select(e => e.Name).ToArray();
    }

    /// <summary>判断用户是否在团队中</summary>
    /// <param name="teamname">团队名</param>
    /// <param name="username">用户名</param>
    /// <returns></returns>
    public Boolean InTeam(String teamname, String username)
    {
        var team = User.FindByName(teamname);
        if (team == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
        return tu != null;
    }

    /// <summary>删除团队</summary>
    /// <param name="name">团队名</param>
    /// <returns></returns>
    public Boolean DeleteTeam(String name)
    {
        var team = User.FindByName(name);
        if (team == null) return false;

        team.Delete();

        return true;
    }

    /// <summary>获取团队列表。分页搜索</summary>
    /// <param name="keyword">搜索关键词</param>
    /// <param name="page">页码</param>
    /// <param name="pagesize">每页大小</param>
    /// <returns></returns>
    public TeamListModel GetTeamList(String keyword, Int32 page, Int32 pagesize = 20)
    {
        var p = new PageParameter
        {
            PageIndex = page,
            PageSize = pagesize,
            RetrieveTotalCount = true,
        };
        var list = User.SearchTeam(keyword, p);

        var model = new TeamListModel
        {
            Teams = list.ToList().Select(s => new TeamModel
            {
                Name = s.Name,
                Nickname = s.NickName,
                Description = s.Description,
            })
                .ToArray(),
            CurrentPage = page,
            ItemCount = (Int32)p.TotalCount,
        };
        return model;
    }
    #endregion
}