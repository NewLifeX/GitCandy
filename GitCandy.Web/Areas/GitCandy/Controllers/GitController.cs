using System.ComponentModel;
using GitCandy.Configuration;
using NewLife.Cube;

namespace GitCandy.Web.Controllers
{
    [DisplayName("糖果配置")]
    public class GitController : ConfigController<UserConfiguration> { }
}