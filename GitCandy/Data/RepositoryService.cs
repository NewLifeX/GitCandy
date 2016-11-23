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
        public Repository Create(RepositoryModel model, Int32 uid, out Boolean badName)
        {
            badName = false;
            var repo = Repository.FindByUserIDAndName(uid, model.Name);
            if (repo != null)
            {
                badName = true;
                return null;
            }

            repo = new Repository
            {
                Name = model.Name,
                Description = model.Description,
                UserID = uid,
                CreateTime = DateTime.UtcNow,
                IsPrivate = model.IsPrivate,
                AllowAnonymousRead = model.AllowAnonymousRead,
                AllowAnonymousWrite = model.AllowAnonymousWrite,
            };
            repo.Save();

            var ur = new UserRepository();
            ur.UserID = uid;
            ur.RepositoryID = repo.ID;
            ur.IsOwner = true;
            ur.Save();

            return repo;
        }

        public RepositoryModel Get(String owner, String reponame, Boolean withShipment = false, String username = null)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
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
                    .Select(s => new { s.User.Name, s.IsOwner, Kind = !s.User.IsTeam })
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

        public Boolean Update(String owner, RepositoryModel model)
        {
            var repo = Repository.FindByOwnerAndName(owner, model.Name);
            if (repo == null) return false;

            repo.IsPrivate = model.IsPrivate;
            repo.AllowAnonymousRead = model.AllowAnonymousRead;
            repo.AllowAnonymousWrite = model.AllowAnonymousWrite;
            repo.Description = model.Description;

            repo.Save();

            return true;
        }

        public CollaborationModel GetRepositoryCollaborationModel(String owner, String name)
        {
            var repo = Repository.FindByOwnerAndName(owner, name);
            if (repo == null) return null;

            var list = UserRepository.FindAllByRepositoryID(repo.ID).ToList();
            var model = new CollaborationModel
            {
                RepositoryName = repo.Name,
                Users = list.Where(e => !e.User.IsTeam)
                    .Select(s => new CollaborationModel.UserRole
                    {
                        Name = s.User.Name,
                        AllowRead = s.AllowRead,
                        AllowWrite = s.AllowWrite,
                        IsOwner = s.IsOwner,
                    })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray(),
                Teams = list.Where(e => e.User.IsTeam)
                    .Select(s => new CollaborationModel.TeamRole
                    {
                        Name = s.User.Name,
                        AllowRead = s.AllowRead,
                        AllowWrite = s.AllowWrite,
                    })
                    .OrderBy(s => s.Name, new StringLogicalComparer())
                    .ToArray(),
            };
            return model;
        }

        public UserRepository RepositoryAddUser(String owner, String reponame, String username)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
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

        public Boolean RepositoryRemoveUser(String owner, String reponame, String username)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role == null) return false;

            role.Delete();

            return true;
        }

        public Boolean RepositoryUserSetValue(String owner, String reponame, String username, String field, Boolean value)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
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

        public Boolean Delete(String owner, String name)
        {
            var repo = Repository.FindByOwnerAndName(owner, name);
            if (repo == null) return false;

            repo.Delete();

            return true;
        }

        public Boolean IsRepositoryAdministrator(String owner, String reponame, String username)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;

            var user = User.FindByName(username);
            if (user == null) return false;

            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role == null) return false;

            return role.IsOwner;
        }

        private Boolean CheckReadWrite(Repository repo, User user, Boolean write)
        {
            //var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && (!write || repo.AllowAnonymousWrite)) return true;

            if (user == null) return false;
            if (user.IsAdmin) return true;

            // 个人权限
            var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
            if (role != null && role.AllowRead && (!write || role.AllowWrite)) return true;

            // 团队权限
            foreach (var item in user.Teams)
            {
                role = UserRepository.FindByUserIDAndRepositoryID(item.TeamID, repo.ID);
                if (role != null && role.AllowRead && (!write || role.AllowWrite)) return true;
            }

            return false;
        }

        public Boolean CanReadRepository(String owner, String reponame, String username)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

            var user = User.FindByName(username);
            if (user == null) return false;
            if (user.IsAdmin) return true;

            return CheckReadWrite(repo, user, false);
        }

        public Boolean CanWriteRepository(String owner, String reponame, String username)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

            var user = User.FindByName(username);
            if (user == null) return false;
            if (user.IsAdmin) return true;

            return CheckReadWrite(repo, user, true);
        }

        public Boolean CanReadRepository(String owner, String reponame, String fingerprint, String publickey)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead) return true;

            var ssh = SshKey.FindByFingerprint(fingerprint);
            if (ssh == null || ssh.PublicKey != publickey) return false;

            var user = ssh.User;
            if (user == null) return false;
            if (user.IsAdmin) return true;

            return CheckReadWrite(repo, user, false);
        }

        public Boolean CanWriteRepository(String owner, String reponame, String fingerprint, String publickey)
        {
            var repo = Repository.FindByOwnerAndName(owner, reponame);
            if (repo == null) return false;
            if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

            var ssh = SshKey.FindByFingerprint(fingerprint);
            if (ssh == null || ssh.PublicKey != publickey) return false;

            var user = ssh.User;
            if (user == null) return false;
            if (user.IsAdmin) return true;

            return CheckReadWrite(repo, user, true);
        }

        public RepositoryListModel GetRepositories(String username, Boolean showAll, PageParameter param)
        {
            var model = new RepositoryListModel();

            // 默认按照最后更新时间降序
            if (param.Sort.IsNullOrEmpty())
            {
                param.Sort = "LastCommit";
                param.Desc = true;
            }

            if (String.IsNullOrEmpty(username))
            {
                model.Collaborations = new RepositoryModel[0];
                model.Repositories = ToRepositoryArray(Repository.GetPublics(param));
            }
            else
            {
                var user = User.FindByName(username);
                var q1 = user.Repositories.Select(e => e.Repository);
                var q2 = user.Teams.SelectMany(s => s.Team.Repositories.Select(e => e.Repository));
                var q3 = q1.Union(q2);
                q3 = q3.OrderByDescending(e => e.LastCommit);

                model.Collaborations = ToRepositoryArray(q3);
                var list = Repository.Search(!showAll, q3.Select(e => e.ID), param);
                model.Repositories = ToRepositoryArray(list);
            }

            return model;
        }

        private RepositoryModel[] ToRepositoryArray(IEnumerable<Repository> source)
        {
            return source.Select(s => new RepositoryModel
            {
                Name = s.Name,
                Description = s.Description,
                Commits = s.Commits,
                Branches = s.Branches,
                Contributors = s.Contributors,
                LastCommit = s.LastCommit,
                Views = s.Views,
                LastView = s.LastView,
            }).ToArray();
        }
    }
}