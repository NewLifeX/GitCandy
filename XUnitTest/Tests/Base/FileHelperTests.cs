using System.Text;
using GitCandy.Base;
using NewLife;

namespace GitCandyXUnitTest.Base;

/// <summary>文件编码探测测试</summary>
public class FileHelperTests
{
    static FileHelperTests() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    [Fact]
    [DisplayName("DetectEncoding_UTF8中文跨1024字节边界_识别为UTF8")]
    public void DetectEncoding_Utf8ChineseAcrossBufferBoundary_ReturnsUtf8()
    {
        // 让首个中文字符正好跨越1024字节的探测缓冲区边界
        var text = new String('a', 1019) + "中文测试内容，用于检查编码探测是否正确识别UTF-8编码的文本文件。";
        var bytes = Encoding.UTF8.GetBytes(text);

        var encoding = FileHelper.DetectEncoding(bytes, Encoding.UTF8, null);

        Assert.NotNull(encoding);
        Assert.Equal(Encoding.UTF8.CodePage, encoding.CodePage);
        Assert.Equal(text, bytes.ToStr(encoding));
    }

    [Fact]
    [DisplayName("DetectEncoding_UTF8无BOM中文_识别为UTF8")]
    public void DetectEncoding_Utf8Chinese_ReturnsUtf8()
    {
        var text = "# 码神工具\n中文说明内容";
        var bytes = Encoding.UTF8.GetBytes(text);

        var encoding = FileHelper.DetectEncoding(bytes, Encoding.UTF8, null);

        Assert.NotNull(encoding);
        Assert.Equal(Encoding.UTF8.CodePage, encoding.CodePage);
        Assert.Equal(text, bytes.ToStr(encoding));
    }

    [Fact]
    [DisplayName("DetectEncoding_GBK中文_识别为GBK")]
    public void DetectEncoding_GbkChinese_ReturnsGbk()
    {
        var text = "# 码神工具\n中文说明内容";
        var encoding936 = Encoding.GetEncoding(936);
        var bytes = encoding936.GetBytes(text);

        var encoding = FileHelper.DetectEncoding(bytes, encoding936, null);

        Assert.NotNull(encoding);
        Assert.Equal(936, encoding.CodePage);
        Assert.Equal(text, bytes.ToStr(encoding));
    }

    [Theory]
    [InlineData(".png", "image/png")]
    [InlineData(".JPG", "image/jpeg")]
    [InlineData(".svg", "image/svg+xml")]
    [InlineData(".ico", "image/x-icon")]
    [InlineData(".bin", FileHelper.BinaryMimeType)]
    [InlineData(null, FileHelper.BinaryMimeType)]
    [DisplayName("GetImageMimeType_按扩展名返回图片类型")]
    public void GetImageMimeType_Extension_ReturnsMimeType(String extension, String expected)
    {
        Assert.Equal(expected, FileHelper.GetImageMimeType(extension));
    }
}
