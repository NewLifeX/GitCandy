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

    [Fact]
    [DisplayName("FixMarkdown_相对链接_改写为仓库内地址")]
    public void FixMarkdown_RelativeLink_RewritesToRepositoryUrl()
    {
        var model = new TreeEntryModel
        {
            Path = "docs/guide.md",
            ReferenceName = "master",
            TextContent = "详见 [用法](usage.md) 与 [目录](sub/)",
        };

        model.FixMarkdown("/team/repo");

        Assert.Contains("(/team/repo/Blob/master/docs/usage.md)", model.TextContent);
        Assert.Contains("(/team/repo/Blob/master/docs/sub/)", model.TextContent);
    }

    [Fact]
    [DisplayName("FixMarkdown_锚点与外部链接_保持原样")]
    public void FixMarkdown_AnchorAndExternalLinks_Unchanged()
    {
        var model = new TreeEntryModel
        {
            Path = "README.md",
            ReferenceName = "master",
            TextContent = "[章节](#install) [站外](https://example.com) [邮件](mailto:a@b.com) [根路径](/Repository)",
        };

        model.FixMarkdown("/team/repo");

        Assert.Contains("](#install)", model.TextContent);
        Assert.Contains("(https://example.com)", model.TextContent);
        Assert.Contains("(mailto:a@b.com)", model.TextContent);
        Assert.Contains("(/Repository)", model.TextContent);
    }

    [Fact]
    [DisplayName("FixMarkdown_带标题链接_保留标题")]
    public void FixMarkdown_LinkWithTitle_KeepsTitle()
    {
        var model = new TreeEntryModel
        {
            Path = "README.md",
            ReferenceName = "master",
            TextContent = "[用法](docs/usage.md \"使用说明\")",
        };

        model.FixMarkdown("/team/repo");

        Assert.Contains("(/team/repo/Blob/master/docs/usage.md \"使用说明\")", model.TextContent);
    }
}
