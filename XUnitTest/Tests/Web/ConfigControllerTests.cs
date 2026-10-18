using System.ComponentModel;
using GitCandy.Web;
using GitCandy.Web.Areas.GitCandy;
using NewLife;
using NewLife.Cube;
using Xunit;
using ConfigControllerX = NewLife.Cube.ConfigController<GitCandy.Web.GitSetting>;

namespace GitCandyXUnitTest.Web;

/// <summary>系统配置测试。覆盖GitSetting默认值与魔方配置控制器接线</summary>
[Collection("GitCandyTests")]
public class ConfigControllerTests
{
    public ConfigControllerTests(DatabaseFixture fixture) { }

    [Fact]
    [DisplayName("GitSetting_默认值_有效")]
    public void GitSetting_Defaults_AreValid()
    {
        var cfg = GitSetting.Current;

        Assert.True(cfg.Commits > 0);
        Assert.True(cfg.PageSize > 0);
        Assert.True(cfg.Contributors > 0);
        Assert.False(cfg.RepositoryPath.IsNullOrEmpty());
        Assert.False(cfg.CachePath.IsNullOrEmpty());
    }

    [Fact]
    [DisplayName("GitController_继承魔方配置控制器")]
    public void GitController_IsConfigController()
    {
        var type = typeof(GitCandy.Web.Areas.GitCandy.Controllers.GitController);

        Assert.True(typeof(ConfigControllerX).IsAssignableFrom(type));
        Assert.NotNull(type.GetCustomAttributes(typeof(GitCandyArea), true).FirstOrDefault());
    }
}
