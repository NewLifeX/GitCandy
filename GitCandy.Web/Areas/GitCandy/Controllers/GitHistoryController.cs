using NewLife.GitCandy.Entity;
using NewLife;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Cube.ViewModels;
using NewLife.Web;
using XCode.Membership;

namespace GitCandy.Web.Areas.GitCandy.Controllers
{
    /// <summary>Git历史</summary>
    [Menu(10, true, Icon = "fa-table")]
    [GitCandyArea]
    public class GitHistoryController : EntityController<GitHistory>
    {
        static GitHistoryController()
        {
            //LogOnChange = true;

            //ListFields.RemoveField("Id", "Creator");
            ListFields.RemoveCreateField();

            //{
            //    var df = ListFields.GetField("Code") as ListField;
            //    df.Url = "?code={Code}";
            //}
            //{
            //    var df = ListFields.AddListField("devices", null, "Onlines");
            //    df.DisplayName = "查看设备";
            //    df.Url = "Device?groupId={Id}";
            //    df.DataVisible = e => (e as GitHistory).Devices > 0;
            //}
            //{
            //    var df = ListFields.GetField("Kind") as ListField;
            //    df.GetValue = e => ((Int32)(e as GitHistory).Kind).ToString("X4");
            //}
            ListFields.TraceUrl("TraceId");
        }

        /// <summary>高级搜索。列表页查询、导出Excel、导出Json、分享页等使用</summary>
        /// <param name="p">分页器。包含分页排序参数，以及Http请求参数</param>
        /// <returns></returns>
        protected override IEnumerable<GitHistory> Search(Pager p)
        {
            var userId = p["userId"].ToInt(-1);
            var repoId = p["repoId"].ToInt(-1);
            var action = p["action"];

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return GitHistory.Search(userId, repoId, action, start, end, p["Q"], p);
        }
    }
}