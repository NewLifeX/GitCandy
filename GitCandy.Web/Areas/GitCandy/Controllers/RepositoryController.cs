using NewLife.Cube;
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
    }

    protected override IEnumerable<Repository> Search(Pager p)
    {
        var ownerId = p["ownerId"].ToInt(-1);
        var userId = p["userId"].ToInt(-1);
        var enable = p["enable"]?.ToBoolean();
        var isPrivate = p["isPrivate"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return Repository.Search(ownerId, userId, enable, isPrivate, start, end, p["q"], p);
    }
}