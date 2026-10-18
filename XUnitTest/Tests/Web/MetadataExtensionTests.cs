using System.ComponentModel;
using GitCandy.Models;
using GitCandy.Web.Extensions;
using Xunit;

namespace GitCandyXUnitTest.Web;

/// <summary>树页面链接生成测试。覆盖特殊字符路径编码</summary>
public class MetadataExtensionTests
{
    [Fact]
    [DisplayName("GetUrl_特殊字符路径_正确编码")]
    public void GetUrl_SpecialPath_EncodesSegments()
    {
        var model = new TreeModel
        {
            Owner = "team",
            Name = "repo",
            ReferenceName = "master",
        };

        var url = model.GetUrl("Tree", "a b#c/中文.txt");

        Assert.Equal("/team/repo/Tree/master/a%20b%23c/%E4%B8%AD%E6%96%87.txt", url);
    }
}
