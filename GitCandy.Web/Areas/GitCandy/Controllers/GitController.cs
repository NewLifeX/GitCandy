using System.ComponentModel;
using NewLife.Cube;

namespace GitCandy.Web.Areas.GitCandy.Controllers
{
    [GitCandyArea]
    [DisplayName("糖果配置")]
    public class GitController : ConfigController<GitSetting> { }
}