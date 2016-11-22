using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using NewLife.Common;
using NewLife.Cube;

namespace GitCandy.Web
{
    [DisplayName("糖果仓库")]
    public class GitCandyAreaRegistration : AreaRegistrationBase
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            base.RegisterArea(context);

            Task.Run(() =>
            {
                var cfg = SysConfig.Current;
                if (cfg.Name == "NewLife.Cube")
                {
                    cfg.Name = "NewLife.GitCandy";
                    cfg.DisplayName = "新生命 糖果仓库";
                    cfg.Save();
                }
            });
        }
    }
}