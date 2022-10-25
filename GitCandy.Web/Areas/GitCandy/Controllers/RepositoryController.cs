using NewLife.Cube;
using NewLife.Cube.ViewModels;
using NewLife.GitCandy.Entity;
using NewLife.Web;

namespace GitCandy.Web.Areas.GitCandy.Controllers;

[GitCandyArea]
public class RepositoryController : EntityController<Repository>
{
    static RepositoryController()
    {
        LogOnChange = true;

        ListFields.RemoveCreateField();
        ListFields.RemoveUpdateField();
        ListFields.RemoveRemarkField();

        {
            var df = ListFields.AddDataField("UserRepositorys", "Commits") as ListField;
            df.DisplayName = "关联团队/用户";
            df.Url = "/GitCandy/UserRepository?repositoryId={ID}";
        }
    }

    protected override IEnumerable<Repository> Search(Pager p)
    {
        var id = p["Id"].ToInt(-1);
        if (id > 0)
        {
            var entity = Repository.FindByKey(id);
            if (entity != null) return new[] { entity };
        }

        var ownerId = p["ownerId"].ToInt(-1);
        var userId = p["userId"].ToInt(-1);
        var enable = p["enable"]?.ToBoolean();
        var isPrivate = p["isPrivate"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return Repository.Search(ownerId, userId, enable, isPrivate, start, end, p["q"], p);
    }
}