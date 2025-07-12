using GitCandy.Base;
using GitCandy.Models;
using NewLife;
using NewLife.Data;
using NewLife.GitCandy.Entity;

namespace GitCandy.Data;

public class RepositoryService
{
    public Repository Create(RepositoryModel model)
    {
        var owner = User.FindByName(model.Owner);
        if (owner == null) throw new ArgumentNullException($"拥有者 {model.Owner} 不存在", "Owner");

        var repo = Repository.FindByOwnerAndName(model.Owner, model.Name);
        if (repo != null) throw new ArgumentException($"{model.Owner} 已存在名为 {model.Name} 的仓库", "Name");

        using (var trans = Repository.Meta.CreateTrans())
        {
            repo = new Repository
            {
                Name = model.Name,
                Description = model.Description,
                OwnerID = owner.ID,
                Enable = true,
                CreateTime = DateTime.UtcNow,
                IsPrivate = model.IsPrivate,
                AllowAnonymousRead = model.AllowAnonymousRead,
                AllowAnonymousWrite = model.AllowAnonymousWrite,
            };
            repo.Save();

            var ur = new UserRepository
            {
                UserID = owner.ID,
                RepositoryID = repo.ID,
                IsOwner = true,
                AllowRead = true,
                AllowWrite = true
            };
            ur.Save();

            trans.Commit();
        }

        return repo;
    }

    public RepositoryModel Get(Repository repo, Boolean withShipment = false, String username = null)
    {
        if (repo == null) return null;

        var model = ToModel(repo);
        if (withShipment || username != null)
        {
            var tempList = UserRepository.FindAllByRepositoryID(repo.ID).ToList().Where(e => e.User != null);

            if (withShipment)
            {
                model.Collaborators = tempList
                    .Where(s => !s.User.IsTeam)
                    .OrderBy(s => s.User.Name)
                    .ToDictionary(e => e.User.Name, e => e.User.NickName);
                model.Teams = tempList
                    .Where(s => s.User.IsTeam)
                    .OrderBy(s => s.User.Name)
                    .ToDictionary(e => e.User.Name, e => e.User.NickName);
            }
            if (username != null)
            {
                model.CurrentUserIsOwner = tempList
                    .Any(s => !s.User.IsTeam && s.IsOwner && s.User.Name == username);
            }
        }
        return model;
    }

    public Boolean Update(Repository repo, RepositoryModel model)
    {
        //var repo = Repository.FindByOwnerAndName(owner, model.Name);
        if (repo == null) return false;

        repo.IsPrivate = model.IsPrivate;
        repo.AllowAnonymousRead = model.AllowAnonymousRead;
        repo.AllowAnonymousWrite = model.AllowAnonymousWrite;
        repo.Description = model.Description;

        repo.Save();

        return true;
    }

    public CollaborationModel GetRepositoryCollaborationModel(Repository repo)
    {
        //var repo = Repository.FindByOwnerAndName(owner, name);
        if (repo == null) return null;

        var list = UserRepository.FindAllByRepositoryID(repo.ID).ToList();
        var model = new CollaborationModel
        {
            Id = repo.ID,
            Name = repo.Name,
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

    public UserRepository RepositoryAddUser(Repository repo, String username)
    {
        //var repo = Repository.FindByOwnerAndName(owner, reponame);
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

    public Boolean RepositoryRemoveUser(Repository repo, String username)
    {
        //var repo = Repository.FindByOwnerAndName(owner, reponame);
        if (repo == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
        if (role == null) return false;

        role.Delete();

        return true;
    }

    public Boolean RepositoryUserSetValue(Repository repo, String username, String field, Boolean value)
    {
        //var repo = Repository.FindByOwnerAndName(owner, reponame);
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

    public Boolean IsRepositoryAdministrator(Repository repo, String username)
    {
        if (repo == null) return false;

        var user = User.FindByName(username);
        if (user == null) return false;

        var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
        if (role == null) return false;

        return role.IsOwner;
    }

    public Boolean IsRepositoryAdministrator(Repository repo, User user)
    {
        if (repo == null) return false;
        if (user == null) return false;

        var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
        if (role == null) return false;

        return role.IsOwner;
    }

    public Boolean CheckReadWrite(Repository repo, User user, Boolean write)
    {
        //var repo = Repository.FindByOwnerAndName(owner, reponame);
        if (repo == null) return false;
        if (repo.AllowAnonymousRead && (!write || repo.AllowAnonymousWrite)) return true;

        if (user == null) return false;
        //if (user.IsAdmin) return true;

        // 个人权限
        var role = UserRepository.FindByUserIDAndRepositoryID(user.ID, repo.ID);
        //if (role != null && (role.IsOwner || role.AllowRead && (!write || role.AllowWrite))) return true;
        if (role != null)
        {
            if (role.IsOwner) return true;
            if (role.AllowRead && (!write || role.AllowWrite)) return true;
        }

        // 团队权限
        foreach (var item in user.Teams)
        {
            role = UserRepository.FindByUserIDAndRepositoryID(item.TeamID, repo.ID);
            if (role != null)
            {
                if (role.IsOwner) return true;
                if (role.AllowRead && (!write || role.AllowWrite)) return true;
            }
        }

        return false;
    }

    public Boolean CanReadRepository(String owner, String reponame, String username)
    {
        var repo = Repository.FindByOwnerAndName(owner, reponame);
        if (repo == null) return false;
        if (repo.AllowAnonymousRead) return true;

        var user = User.FindByName(username);
        if (user == null) return false;
        //if (user.IsAdmin) return true;

        return CheckReadWrite(repo, user, false);
    }

    public Boolean CanReadRepository(Repository repo, User user)
    {
        if (repo == null) return false;
        if (repo.AllowAnonymousRead) return true;

        if (user == null) return false;

        return CheckReadWrite(repo, user, false);
    }

    public Boolean CanWriteRepository(String owner, String reponame, String username)
    {
        var repo = Repository.FindByOwnerAndName(owner, reponame);
        if (repo == null) return false;
        if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

        var user = User.FindByName(username);
        if (user == null) return false;
        //if (user.IsAdmin) return true;

        return CheckReadWrite(repo, user, true);
    }

    //public Boolean CanReadRepository(String owner, String reponame, String fingerprint, String publickey)
    //{
    //    var repo = Repository.FindByOwnerAndName(owner, reponame);
    //    if (repo == null) return false;
    //    if (repo.AllowAnonymousRead) return true;

    //    var ssh = SshKey.FindByFingerprint(fingerprint);
    //    if (ssh == null || ssh.PublicKey != publickey) return false;

    //    var user = ssh.User;
    //    if (user == null) return false;
    //    if (user.IsAdmin) return true;

    //    return CheckReadWrite(repo, user, false);
    //}

    //public Boolean CanWriteRepository(String owner, String reponame, String fingerprint, String publickey)
    //{
    //    var repo = Repository.FindByOwnerAndName(owner, reponame);
    //    if (repo == null) return false;
    //    if (repo.AllowAnonymousRead && repo.AllowAnonymousWrite) return true;

    //    var ssh = SshKey.FindByFingerprint(fingerprint);
    //    if (ssh == null || ssh.PublicKey != publickey) return false;

    //    var user = ssh.User;
    //    if (user == null) return false;
    //    if (user.IsAdmin) return true;

    //    return CheckReadWrite(repo, user, true);
    //}

    public RepositoryListModel GetRepositories(User user, Boolean showAll, PageParameter param)
    {
        var model = new RepositoryListModel();

        // 默认按照最后更新时间降序
        if (param.Sort.IsNullOrEmpty())
        {
            param.Sort = "LastCommit";
            param.Desc = true;
        }

        if (user == null)
        {
            model.Collaborations = [];
            model.Repositories = ToModels(Repository.GetPublics(param));
        }
        else
        {
            //var user = User.FindByName(username);
            //if (user == null) return model;

            //var q1 = user.Repositories.Select(e => e.Repository);
            //var q2 = user.Teams.Where(e => e.Team != null).SelectMany(s => s.Team.Repositories.Select(e => e.Repository));
            //var q3 = q1.Union(q2).Where(e => e.Enable);
            //q3 = q3.OrderByDescending(e => e.LastCommit);

            //model.Collaborations = ToModels(q3);

            //var list = Repository.Search(showAll, q3.Select(e => e.ID).ToArray(), param);

            // 一次性查出来，再区分是否协作者
            //var list = Repository.Search(showAll, null, param);

            // 已登录时，主屏显示当前用户关联仓库（自己和团队的），右边显示非当前用户关联的公开库
            // 未登录时，主屏显示所有用户公开的仓库，右边不显示
            var myRepos = user != null ?
                Repository.Search(user.ID, -1, null, null, param) :
                Repository.Search(-1, -1, true, false, param);

            var otherRepose = user != null ?
                Repository.Search(-1, user.ID, true, false, new PageParameter { PageSize = 30 }) :
                null;

            //var myRepos = new List<Repository>();
            //var otherRepose = new List<Repository>();
            //var userRepoIds = user.Repositories.Select(e => e.RepositoryID).ToArray();
            //var teamRepoIds = user.Teams.Where(e => e.Team != null).SelectMany(s => s.Team.Repositories.Select(e => e.RepositoryID)).ToArray();
            //foreach (var item in list)
            //{
            //    if (userRepoIds.Contains(item.ID) || teamRepoIds.Contains(item.ID))
            //        myRepos.Add(item);
            //    else
            //        otherRepose.Add(item);
            //}

            model.Collaborations = ToModels(myRepos);
            model.Repositories = ToModels(otherRepose);
            model.CurrentPage = param.PageIndex;
            model.ItemCount = (Int32)param.TotalCount;
        }

        return model;
    }

    private RepositoryModel[] ToModels(IEnumerable<Repository> source) => source.Select(ToModel).ToArray();

    private RepositoryModel ToModel(Repository repo)
    {
        return new RepositoryModel
        {
            Id = repo.ID,
            Owner = repo.Owner.Name,
            Name = repo.Name,
            Description = repo.Description,
            IsPrivate = repo.IsPrivate,
            Commits = repo.Commits,
            Branches = repo.Branches,
            Contributors = repo.Contributors,
            LastCommit = repo.LastCommit,
            Views = repo.Views,
            LastView = repo.LastView,
            Downloads = repo.Downloads
        };
    }
}