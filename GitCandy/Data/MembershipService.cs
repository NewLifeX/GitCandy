using System;
using System.Linq;
using GitCandy.Base;
using GitCandy.Models;
using GitCandy.Security;
using GitCandy.Ssh;
using NewLife.Data;
using NewLife.GitCandy.Entity;

namespace GitCandy.Data
{
    public class MembershipService
    {
        #region Account part
        public UserModel GetUserModel(String name, bool withMembers = false, String viewUser = null)
        {
            var user = User.FindByName(name);
            if (user == null) return null;

            var model = new UserModel
            {
                Name = user.Name,
                Nickname = user.Nickname,
                Email = user.Email,
                Description = user.Description,
                IsSystemAdministrator = user.IsAdmin,
            };
            if (withMembers)
            {
                model.Teams = user.TeamNames;
                var rs = user.Repositories.Select(e => e.Repository).Where(e => e.Enable);
                if (!viewUser.IsNullOrEmpty())
                {
                    var vu = User.FindByName(viewUser);
                    rs = rs.Where(e => e.CanViewFor(vu));
                }
                model.Respositories = rs.Select(e => e.Name).OrderBy(e => e).ToArray();
            }
            return model;
        }

        public User Login(String id, String password)
        {
            var user = User.FindByName(id) ?? User.FindByEmail(id);
            if (user != null && user.Login(password)) return user;

            return null;
        }

        public void SetPassword(String name, String newPassword)
        {
            var user = User.FindByName(name);
            if (user != null)
            {
                user.Password = newPassword.MD5();

                var auths = AuthorizationLog.FindAllByUserID(user.ID);
                foreach (var auth in auths)
                {
                    auth.IsValid = false;
                }
                user.Save();
                auths.Save();
            }
        }

        public bool UpdateUser(UserModel model)
        {
            var user = User.FindByName(model.Name);
            if (user != null)
            {
                user.Nickname = model.Nickname;
                user.Email = model.Email;
                user.Description = model.Description;
                user.IsAdmin = model.IsSystemAdministrator;

                user.Save();
                return true;
            }
            return false;
        }

        public AuthorizationLog CreateAuthorization(long userID, DateTime expires, String ip)
        {
            var auth = new AuthorizationLog
            {
                AuthCode = Guid.NewGuid().ToString(),
                UserID = (Int32)userID,
                IssueDate = DateTime.Now,
                Expires = expires,
                IssueIp = ip,
                LastIp = ip,
                IsValid = true,
            };
            auth.Save();
            return auth;
        }

        public Token GetToken(Guid authCode)
        {
            var auth = AuthorizationLog.FindByAuthCode(authCode + "");
            if (auth == null) return null;

            var user = auth.User;

            return new Token(auth.AuthCode, auth.ID, user.Name, user.Nickname, user.IsAdmin, auth.Expires)
            {
                LastIp = auth.LastIp
            };
        }

        public void UpdateAuthorization(Guid authCode, DateTime expires, String lastIp)
        {
            var auth = AuthorizationLog.FindByAuthCode(authCode + "");
            if (auth != null)
            {
                auth.Expires = expires;
                auth.LastIp = lastIp;
                auth.Save();
            }
        }

        public void SetAuthorizationAsInvalid(Guid authCode)
        {
            var auth = AuthorizationLog.FindByAuthCode(authCode + "");
            if (auth != null)
            {
                auth.IsValid = false;
                auth.Save();
            }
        }

        public void DeleteUser(String name)
        {
            var user = User.FindByName(name);
            if (user != null) user.Delete();
        }

        public UserListModel GetUserList(String keyword, int page, int pagesize = 20)
        {
            var p = new PageParameter();
            p.PageIndex = page;
            p.PageSize = pagesize;
            var list = User.SearchByName(keyword, p);

            return new UserListModel
            {
                Users = list.ToList().Select(e => new UserModel
                {
                    Name = e.Name,
                    Nickname = e.Nickname,
                    Email = e.Email,
                    Description = e.Description,
                    IsSystemAdministrator = e.IsAdmin,
                }).ToArray(),
                CurrentPage = page,
                ItemCount = p.TotalCount
            };
        }

        public String AddSshKey(String name, String sshkey)
        {
            var seg = sshkey.Split();
            var type = seg[0];
            sshkey = seg[1];
            var fingerprint = KeyUtils.GetFingerprint(sshkey);

            var user = User.FindByName(name);
            if (user == null) return null;

            var key = new SshKey
            {
                UserID = user.ID,
                KeyType = type,
                Fingerprint = fingerprint,
                PublicKey = sshkey,
                ImportData = DateTime.UtcNow,
                LastUse = DateTime.UtcNow,
            };

            key.Save();

            return fingerprint;
        }

        public void DeleteSshKey(String name, String sshkey)
        {
            var user = User.FindByName(name);
            if (user == null) return;

            var key = SshKey.FindByUserID(user.ID);
            if (key == null) return;

            if (key.Fingerprint == sshkey) key.Delete();
        }

        public bool HasSshKey(String fingerprint)
        {
            return SshKey.FindByFingerprint(fingerprint) != null;
        }

        public SshModel GetSshList(String name)
        {
            var user = User.FindByName(name);
            if (user == null) return null;

            return new Models.SshModel
            {
                Username = user.Name,
                SshKeys = user.SshKeys.Select(s => new SshModel.SshKey { Name = s.Fingerprint }).ToArray()
            };
        }
        #endregion

        #region Team part
        public bool UpdateTeam(TeamModel model)
        {
            var team = User.FindByName(model.Name);
            if (team == null) return false;

            team.Nickname = model.Nickname;
            team.Description = model.Description;
            team.Save();

            return true;
        }

        public TeamModel GetTeamModel(String name, bool withMembers = false, String viewUser = null)
        {
            var team = User.FindByName(name);
            if (team == null) return null;

            var model = new TeamModel
            {
                Name = team.Name,
                Nickname = team.Nickname,
                Description = team.Description,
            };
            if (withMembers)
            {
                model.MembersRole = UserTeam.FindAllByTeamID(team.ID).ToList()
                    .Where(e => e.User != null)
                    .Select(s => new TeamModel.UserRole
                    {
                        Name = s.User.Name,
                        IsAdministrator = s.IsAdmin
                    })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray();
                model.Members = model.MembersRole
                    .Select(s => s.Name)
                    .ToArray();

                var rs = team.Repositories.Where(e => e.Repository.Enable);
                if (!viewUser.IsNullOrEmpty())
                {
                    var vu = User.FindByName(viewUser);
                    rs = rs.Where(e => e.Repository.CanViewFor(vu));
                }
                model.RepositoriesRole = rs.Select(s => new TeamModel.RepositoryRole
                {
                    Name = s.RepositoryName,
                    AllowRead = s.AllowRead,
                    AllowWrite = s.AllowWrite,
                })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray();
                model.Repositories = model.RepositoriesRole
                    .Select(s => s.Name)
                    .ToArray();
            }
            return model;
        }

        public bool TeamAddUser(String teamname, String username)
        {
            var team = User.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null)
            {
                tu = new UserTeam();
                tu.UserID = user.ID;
                tu.TeamID = team.ID;
                tu.Save();
            }

            return true;
        }

        public bool TeamRemoveUser(String teamname, String username)
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

        public bool TeamUserSetAdministrator(String teamname, String username, bool isAdmin)
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

        public String[] SearchTeam(String query)
        {
            var p = new PageParameter();
            var list = User.SearchTeam(query, p);
            return list.ToList().Select(e => e.Name).ToArray();
        }

        public bool IsTeamAdministrator(String teamname, String username)
        {
            var team = User.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null) return false;

            return tu.IsAdmin;
        }

        public Boolean DeleteTeam(String name)
        {
            var team = User.FindByName(name);
            if (team == null) return false;

            team.Delete();

            return true;
        }

        public TeamListModel GetTeamList(String keyword, int page, int pagesize = 20)
        {
            var p = new PageParameter();
            p.PageIndex = page;
            p.PageSize = pagesize;
            var list = User.SearchTeam(keyword, p);

            var model = new TeamListModel
            {
                Teams = list.ToList().Select(s => new TeamModel
                {
                    Name = s.Name,
                    Nickname = s.Nickname,
                    Description = s.Description,
                })
                    .ToArray(),
                CurrentPage = page,
                ItemCount = p.TotalCount,
            };
            return model;
        }
        #endregion
    }
}