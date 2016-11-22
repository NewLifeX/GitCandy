using System;
using System.Collections.Generic;
using System.Linq;
using GitCandy.Base;
using GitCandy.Models;
using NewLife.Data;
using NewLife.GitCandy.Entity;

namespace GitCandy.Data
{
    public class RepositoryService
    {
        public Repository Create(RepositoryModel model, long managerID, out bool badName)
        {
            badName = false;
            var rp = Repository.FindByName(model.Name);
            if (rp != null)
            {
                badName = true;
                return null;
            }

            rp = new Repository
            {
                Name = model.Name,
                Description = model.Description,
                CreateTime = DateTime.UtcNow,
                IsPrivate = model.IsPrivate,
                AllowAnonymousRead = model.AllowAnonymousRead,
                AllowAnonymousWrite = model.AllowAnonymousWrite,
            };
            rp.Save();

            if (managerID > 0)
            {
                var role = new UserRepository
                {
                    RepositoryID = rp.ID,
                    UserID = (Int32)managerID,
                    IsOwner = true,
                    AllowRead = true,
                    AllowWrite = true
                };
                role.Save();
            }

            return rp;
        }

        public RepositoryModel GetRepositoryModel(string reponame, bool withShipment = false, string username = null)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return null;

            var model = new RepositoryModel
            {
                Name = repo.Name,
                Description = repo.Description,
                IsPrivate = repo.IsPrivate,
                AllowAnonymousRead = repo.AllowAnonymousRead,
                AllowAnonymousWrite = repo.AllowAnonymousWrite,
            };
            if (withShipment || username != null)
            {
                var tempList = UserRepository.FindAllByRepositoryID(repo.ID).ToList()
                    .Select(s => new { s.User.Name, s.IsOwner, Kind = true })
                    .Concat(TeamRepository.FindAllByRepositoryID(repo.ID).ToList()
                        .Select(s => new { s.Team.Name, IsOwner = false, Kind = false }))
                    .ToList();

                if (withShipment)
                {
                    model.Collaborators = tempList
                        .Where(s => s.Kind)
                        .Select(s => s.Name)
                        .OrderBy(s => s, new StringLogicalComparer())
                        .ToArray();
                    model.Teams = tempList
                        .Where(s => !s.Kind)
                        .Select(s => s.Name)
                        .OrderBy(s => s, new StringLogicalComparer())
                        .ToArray();
                }
                if (username != null)
                {
                    model.CurrentUserIsOwner = tempList
                        .Any(s => s.Kind && s.IsOwner && s.Name == username);
                }
            }
            return model;
        }

        public bool Update(RepositoryModel model)
        {
            var repo = Repository.FindByName(model.Name);
            if (repo == null) return false;

            repo.IsPrivate = model.IsPrivate;
            repo.AllowAnonymousRead = model.AllowAnonymousRead;
            repo.AllowAnonymousWrite = model.AllowAnonymousWrite;
            repo.Description = model.Description;

            repo.Save();

            return true;
        }

        public CollaborationModel GetRepositoryCollaborationModel(string name)
        {
            var repo = Repository.FindByName(name);
            if (repo == null) return null;

            var model = new CollaborationModel
            {
                RepositoryName = repo.Name,
                Users = UserRepository.FindAllByRepositoryID(repo.ID).ToList()
                    .Select(s => new CollaborationModel.UserRole
                    {
                        Name = s.User.Name,
                        AllowRead = s.AllowRead,
                        AllowWrite = s.AllowWrite,
                        IsOwner = s.IsOwner,
                    })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray(),
                Teams = TeamRepository.FindAllByRepositoryID(repo.ID).ToList()
                    .Select(s => new CollaborationModel.TeamRole
                    {
                        Name = s.Team.Name,
                        AllowRead = s.AllowRead,
                        AllowWrite = s.AllowWrite,
                    })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray(),
            };
            return model;
        }

        public UserRepository RepositoryAddUser(string reponame, string username)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return null;

            var user = User.FindByName(username);
            if (user == null) return null;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null) return role;

            role = new UserRepository
            {
                RepositoryID = repo.ID,
                UserID = user.ID,
                AllowRead = true,
                AllowWrite = true,
                IsOwner = false,
            };
            role.Save();

            return role;
        }

        public bool RepositoryRemoveUser(string reponame, string username)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role == null) return false;

            role.Delete();

            return true;
        }

        public bool RepositoryUserSetValue(string reponame, string username, string field, bool value)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role == null) return false;

            if (field == "read")
                role.AllowRead = value;
            else if (field == "write")
                role.AllowWrite = value;
            else if (field == "owner")
                role.IsOwner = value;
            else
                return false;

            role.Save();

            return true;
        }

        public TeamRepository RepositoryAddTeam(string reponame, string teamname)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return null;

            var team = Team.FindByName(teamname);
            if (team == null) return null;

            var role = TeamRepository.FindByTeamIDAndRepositoryID(team.ID, repo.ID);
            if (role != null) return role;

            role = new TeamRepository
            {
                RepositoryID = repo.ID,
                TeamID = team.ID,
                AllowRead = true,
                AllowWrite = true,
            };

            role.Save();

            return role;
        }

        public bool RepositoryRemoveTeam(string reponame, string teamname)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;

            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var role = TeamRepository.FindByTeamIDAndRepositoryID(team.ID, repo.ID);
            if (role == null) return false;

            role.Delete();

            return true;
        }

        public bool RepositoryTeamSetValue(string reponame, string teamname, string field, bool value)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;

            var team = Team.FindByName(teamname);
            if (team == null) return false;

            var role = TeamRepository.FindByTeamIDAndRepositoryID(team.ID, repo.ID);
            if (role == null) return false;

            if (field == "read")
                role.AllowRead = value;
            else if (field == "write")
                role.AllowWrite = value;
            else
                return false;

            role.Save();

            return true;
        }

        public Boolean Delete(string name)
        {
            var repo = Repository.FindByName(name);
            if (repo == null) return false;

            repo.Delete();

            return true;
        }

        public bool IsRepositoryAdministrator(string reponame, string username)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role == null) return false;

            return role.IsOwner;
        }

        public bool CanReadRepository(string reponame, string username)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead) return true;

            var user = User.FindByName(username);
            if (user == null) return false;
            if (user.IsAdmin) return true;

            // 个人权限
            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null && role.AllowRead) return true;

            // 团队权限
            var ts = user.Teams.Select(e => e.ID).ToArray();
            if (ts.Length == 0) return false;

            var roles = TeamRepository.FindAllByRepositoryID(repo.ID);
            foreach (var item in roles)
            {
                if (item.AllowRead && ts.Contains(item.TeamID)) return true;
            }

            return false;
        }

        public bool CanWriteRepository(string reponame, string username)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

            var user = User.FindByName(username);
            if (user == null) return false;
            if (user.IsAdmin) return true;

            // 个人权限
            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null && role.AllowRead && role.AllowWrite) return true;

            // 团队权限
            var ts = user.Teams.Select(e => e.ID).ToArray();
            if (ts.Length == 0) return false;

            var roles = TeamRepository.FindAllByRepositoryID(repo.ID);
            foreach (var item in roles)
            {
                if (item.AllowRead && item.AllowWrite && ts.Contains(item.TeamID)) return true;
            }

            return false;
        }

        public bool CanReadRepository(string reponame, string fingerprint, string publickey)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead) return true;

            var ssh = SshKey.FindByFingerprint(fingerprint);
            if (ssh == null || ssh.PublicKey != publickey) return false;

            var user = ssh.User;
            if (user == null) return false;
            if (user.IsAdmin) return true;

            // 个人权限
            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null && role.AllowRead) return true;

            // 团队权限
            var ts = user.Teams.Select(e => e.ID).ToArray();
            if (ts.Length == 0) return false;

            var roles = TeamRepository.FindAllByRepositoryID(repo.ID);
            foreach (var item in roles)
            {
                if (item.AllowRead && ts.Contains(item.TeamID)) return true;
            }

            return false;
        }

        public bool CanWriteRepository(string reponame, string fingerprint, string publickey)
        {
            var repo = Repository.FindByName(reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

            var ssh = SshKey.FindByFingerprint(fingerprint);
            if (ssh == null || ssh.PublicKey != publickey) return false;

            var user = ssh.User;
            if (user == null) return false;
            if (user.IsAdmin) return true;

            // 个人权限
            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null && role.AllowRead && role.AllowWrite) return true;

            // 团队权限
            var ts = user.Teams.Select(e => e.ID).ToArray();
            if (ts.Length == 0) return false;

            var roles = TeamRepository.FindAllByRepositoryID(repo.ID);
            foreach (var item in roles)
            {
                if (item.AllowRead && item.AllowWrite && ts.Contains(item.TeamID)) return true;
            }

            return false;
        }

        public RepositoryListModel GetRepositories(string username, bool showAll, PageParameter param)
        {
            var model = new RepositoryListModel();

            if (string.IsNullOrEmpty(username))
            {
                model.Collaborations = new RepositoryModel[0];
                model.Repositories = ToRepositoryArray(Repository.GetPublics(param));
            }
            else
            {
                var user = User.FindByName(username);
                var q1 = UserRepository.FindAllByUserID(user.ID).ToList().Where(s => s.AllowRead && s.AllowWrite).Select(s => s.Repository);
                var q2 = user.Teams.SelectMany(s => TeamRepository.FindAllByTeamID(s.ID).ToList().Where(t => t.AllowRead && t.AllowWrite).Select(t => t.Repository));
                var q3 = q1.Union(q2);

                model.Collaborations = ToRepositoryArray(q3);
                //model.Repositories = ToRepositoryArray(ctx.Repositories.Where(s => showAll || (!s.IsPrivate)).Except(q3).OrderBy(s => s.Name));
                var exp = Repository._.ID.NotIn(q3.Select(e => e.ID));
                if (!showAll) exp &= Repository._.IsPrivate.IsTrue(false);
                var list = Repository.FindAll(exp, param).ToList();
                model.Repositories = ToRepositoryArray(list.ToList());
            }

            return model;
        }

        private RepositoryModel[] ToRepositoryArray(IEnumerable<Repository> source)
        {
            return source.Select(s => new RepositoryModel
            {
                Name = s.Name,
                Description = s.Description,
            }).ToArray();
        }
    }
}