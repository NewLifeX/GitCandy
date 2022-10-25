using NewLife.Cube;
using NewLife.Cube.ViewModels;
using NewLife.GitCandy.Entity;
using NewLife.Web;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Areas.GitCandy.Controllers;

[GitCandyArea]
public class UserController : EntityController<User>
{
    static UserController()
    {
        LogOnChange = true;

        ListFields.RemoveCreateField();
        ListFields.RemoveUpdateField();
        ListFields.RemoveRemarkField();

        {
            var df = ListFields.AddDataField("members", null, "IsTeam") as ListField;
            df.DisplayName = "成员列表";
            df.Url = "/GitCandy/User?ownerId={ID}";
            df.DataVisible = e => (e as UserX).IsTeam;
        }
    }

    protected override IEnumerable<User> Search(Pager p)
    {
        var ownerId = p["ownerId"].ToInt(-1);
        var enable = p["enable"]?.ToBoolean();
        var isTeam = p["isTeam"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return UserX.Search(ownerId, enable, isTeam, start, end, p["q"], p);
    }
}