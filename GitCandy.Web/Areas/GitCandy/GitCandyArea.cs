using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace GitCandy.Web.Areas.GitCandy;

[DisplayName("糖果仓库")]
[Menu(-1, true, Icon = "fa-desktop")]
public class GitCandyArea : AreaBase
{
    public GitCandyArea() : base(nameof(GitCandyArea).TrimEnd("Area")) { }
}