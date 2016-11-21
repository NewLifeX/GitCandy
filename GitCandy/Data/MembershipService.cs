using System;
using System.Composition;
using System.Linq;
using GitCandy.Base;
using GitCandy.Models;
using GitCandy.Security;
using GitCandy.Ssh;
using NewLife.Data;
using NewLife.GitCandy.Entity;
using NewLife.Web;

namespace GitCandy.Data
{
    [Export(typeof(MembershipService))]
    public class MembershipService
    {
        #region Account part
        public User CreateAccount(string name, string nickname, string password, string email, string description, out bool badName, out bool badEmail)
        {
            badName = false;
            badEmail = false;

            var user = User.FindByName(name);
            if (user != null)
            {
                badName = true;
                return null;
            }

            user = User.FindByName(email);
            if (user != null)
            {
                badEmail = true;
                return null;
            }


            user = new User
            {
                Name = name,
                Nickname = nickname,
                Email = email,
                PasswordVersion = -1,
                Password = "",
                Description = description,
                CreateTime = DateTime.Now,
            };

            using (var pp = PasswordProviderPool.Take())
            {
                user.PasswordVersion = pp.Version;
                user.Password = pp.Compute(user.ID, name, password);
            }

            user.Save();

            return user;
        }

        public UserModel GetUserModel(string name, bool withMembers = false, string viewUser = null)
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
                var rs = user.Repositories.Where(e => e.IsOwner);
                if (!viewUser.IsNullOrEmpty())
                {
                    var vu = User.FindByName(viewUser);
                    rs = rs.Where(e => e.Repository != null && e.Repository.CanViewFor(vu));
                }
                model.Respositories = rs.Select(e => e.RepositoryName).OrderBy(e => e).ToArray();
                //model.Respositories = ctx.UserRepositoryRoles
                //    // belong user
                //    .Where(s => s.User.ID == user.ID && s.IsOwner)
                //    // can view for viewUser
                //    .Where(s => !s.Repository.IsPrivate
                //        || viewUser != null &&
                //            (ctx.Users.Any(t => t.Name == viewUser && t.IsSystemAdministrator)
                //            || ctx.UserRepositoryRoles.Any(t => t.RepositoryID == s.RepositoryID
                //                && t.User.Name == viewUser
                //                && t.AllowRead)
                //            || ctx.TeamRepositoryRoles.Any(t => t.RepositoryID == s.RepositoryID
                //                && t.Team.UserTeamRoles.Any(r => r.User.Name == viewUser)
                //                && t.AllowRead)))
                //    .Select(s => s.Repository.Name)
                //    .AsEnumerable()
                //    .OrderBy(s => s, new StringLogicalComparer())
                //    .ToArray();
            }
            return model;
        }

        public User Login(string id, string password)
        {
            var user = User.FindByName(id) ?? User.FindByEmail(id);
            if (user != null)
            {
                using (var pp1 = PasswordProviderPool.Take(user.PasswordVersion))
                    if (user.Password == pp1.Compute(user.ID, user.Name, password))
                    {
                        if (user.PasswordVersion != PasswordProviderPool.LastVersion)
                            using (var pp2 = PasswordProviderPool.Take())
                            {
                                user.Password = pp2.Compute(user.ID, user.Name, password);
                                user.PasswordVersion = pp2.Version;
                                user.Logins++;
                                user.LastLogin = DateTime.Now;
                                user.LastLoginIP = WebHelper.UserHost;
                                user.Save();
                            }
                        return user;
                    }
            }
            return null;
        }

        public void SetPassword(string name, string newPassword)
        {
            var user = User.FindByName(name);
            if (user != null)
            {
                using (var pp = PasswordProviderPool.Take())
                {
                    user.Password = pp.Compute(user.ID, user.Name, newPassword);
                    user.PasswordVersion = pp.Version;
                }

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

        public AuthorizationLog CreateAuthorization(long userID, DateTime expires, string ip)
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

        public void UpdateAuthorization(Guid authCode, DateTime expires, string lastIp)
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

        public void DeleteUser(string name)
        {
            var user = User.FindByName(name);
            if (user != null)
            {
                //user.UserTeamRoles.Clear();
                //user.UserRepositoryRoles.Clear();
                //user.AuthorizationLogs.Clear();
                //user.SshKeys.Clear();

                user.Delete();
            }
        }

        public UserListModel GetUserList(string keyword, int page, int pagesize = 20)
        {
            var p = new PageParameter();
            p.PageIndex = page;
            p.PageSize = pagesize;
            var list = User.Search(keyword, p);

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

            //using (var ctx = new GitCandyContext())
            //{
            //    var query = ctx.Users.AsQueryable();
            //    if (!string.IsNullOrEmpty(keyword))
            //        query = query.Where(s => s.Name.Contains(keyword)
            //            || s.Nickname.Contains(keyword)
            //            || s.Email.Contains(keyword)
            //            || s.Description.Contains(keyword));
            //    query = query.OrderBy(s => s.Name);

            //    var model = new UserListModel
            //    {
            //        Users = query
            //            .Skip((page - 1) * pagesize)
            //            .Take(pagesize)
            //            .Select(user => new UserModel
            //            {
            //                Name = user.Name,
            //                Nickname = user.Nickname,
            //                Email = user.Email,
            //                Description = user.Description,
            //                IsSystemAdministrator = user.IsSystemAdministrator,
            //            })
            //            .ToArray(),
            //        CurrentPage = page,
            //        ItemCount = query.Count(),
            //    };
            //    return model;
            //}
        }

        public string[] SearchUsers(string query)
        {
            var list = User.FindAll(User._.Name.Contains(query), null, null, 0, 10);
            return list.ToList().Select(e => e.Name).ToArray();

            //using (var ctx = new GitCandyContext())
            //{
            //    var length = query.Length + 0.5;
            //    return ctx.Users
            //        .Where(s => s.Name.Contains(query))
            //        .OrderByDescending(s => length / s.Name.Length)
            //        .ThenBy(s => s.Name)
            //        .Take(10)
            //        .Select(s => s.Name)
            //        .ToArray();
            //}
        }

        public string AddSshKey(string name, string sshkey)
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

        public void DeleteSshKey(string name, string sshkey)
        {
            var user = User.FindByName(name);
            if (user == null) return;

            var key = SshKey.FindByUserID(user.ID);
            if (key == null) return;

            if (key.Fingerprint == sshkey) key.Delete();

            //using (var ctx = new GitCandyContext())
            //{
            //    var key = ctx.SshKeys.FirstOrDefault(s => s.User.Name == name && s.Fingerprint == sshkey);
            //    ctx.SshKeys.Remove(key);
            //    ctx.SaveChanges();
            //}
        }

        public bool HasSshKey(string fingerprint)
        {
            //using (var ctx = new GitCandyContext())
            //{
            //    return ctx.SshKeys.Any(s => s.Fingerprint == fingerprint);
            //}

            return SshKey.FindCount(SshKey._.Fingerprint, fingerprint) > 0;
        }

        public SshModel GetSshList(string name)
        {
            //using (var ctx = new GitCandyContext())
            //{
            //    var keys = ctx.SshKeys
            //        .Where(s => s.User.Name == name)
            //        .Select(s => new SshModel.SshKey { Name = s.Fingerprint })
            //        .ToArray();

            //    return new SshModel { Username = name, SshKeys = keys };
            //}

            var user = User.FindByName(name);
            if (user == null) return null;

            return new Models.SshModel { Username = user.Name, SshKeys = user.SshKeys.Select(s => new SshModel.SshKey { Name = s.Fingerprint }).ToArray() };
        }
        #endregion

        #region Team part
        public Team CreateTeam(string name, string description, long managerID, out bool badName)
        {
            badName = false;

            var team = Team.FindByName(name);
            if (team != null)
            {
                badName = true;
                return null;
            }

            team = new Team
            {
                Name = name,
                Description = description,
                //CreationDate = DateTime.UtcNow,
            };
            team.Save();

            if (managerID > 0)
            {
                UserTeam.Add((Int32)managerID, team.ID, true);
                //team.UserTeamRoles.Add(new UserTeamRole { Team = team, UserID = managerID, IsAdministrator = true });
            }
            //ctx.SaveChanges();

            return team;
        }

        public bool UpdateTeam(TeamModel model)
        {
            //using (var ctx = new GitCandyContext())
            //{
            //    var team = ctx.Teams.FirstOrDefault(s => s.Name == model.Name);
            //    if (team != null)
            //    {
            //        team.Description = model.Description;
            //        ctx.SaveChanges();
            //        return true;
            //    }
            //    return false;
            //}

            var team = Team.FindByName(model.Name);
            if (team == null) return false;

            team.Description = model.Description;
            team.Save();

            return true;
        }

        public TeamModel GetTeamModel(string name, bool withMembers = false, string viewUser = null)
        {
            var team = Team.FindByName(name);
            if (team == null) return null;

            var model = new TeamModel
            {
                Name = team.Name,
                Description = team.Description,
            };
            if (withMembers)
            {
                model.MembersRole = UserTeam.FindAllByTeamID(team.ID).ToList()
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

                var rs = team.Repositories.AsEnumerable();
                if (!viewUser.IsNullOrEmpty())
                {
                    var vu = User.FindByName(viewUser);
                    rs = rs.Where(e => e.Repository != null && e.Repository.CanViewFor(vu));
                }
                model.RepositoriesRole = rs.Select(s => new TeamModel.RepositoryRole
                {
                    Name = s.Repository.Name,
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

        public bool TeamAddUser(string teamname, string username)
        {
            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null)
            {
                tu = new NewLife.GitCandy.Entity.UserTeam();
                tu.UserID = user.ID;
                tu.TeamID = team.ID;
                tu.Save();
            }

            return true;
        }

        public bool TeamRemoveUser(string teamname, string username)
        {
            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null) return false;

            tu.Delete();

            return true;
        }

        public bool TeamUserSetAdministrator(string teamname, string username, bool isAdmin)
        {
            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null) return false;

            tu.IsAdmin = true;
            tu.Save();

            return true;
        }

        public string[] SearchTeam(string query)
        {
            var list = Team.FindAll(Team._.Name.Contains(query), null, null, 0, 10);
            return list.ToList().Select(e => e.Name).ToArray();
        }

        public bool IsTeamAdministrator(string teamname, string username)
        {
            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var tu = UserTeam.FindByUserIDAndTeamID(user.ID, team.ID);
            if (tu == null) return false;

            return tu.IsAdmin;
        }

        public Boolean DeleteTeam(string name)
        {
            var team = Team.FindByName(name);
            if (team == null) return false;

            team.Delete();

            return true;
        }

        public TeamListModel GetTeamList(string keyword, int page, int pagesize = 20)
        {
            var p = new PageParameter();
            p.PageIndex = page;
            p.PageSize = pagesize;
            var list = Team.Search(keyword, p);

            var model = new TeamListModel
            {
                Teams = list.ToList().Select(s => new TeamModel
                {
                    Name = s.Name,
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