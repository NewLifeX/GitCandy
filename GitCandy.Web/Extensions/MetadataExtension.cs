using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using GitCandy.Models;
using GitCandy.Web.App_GlobalResources;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewLife;

namespace GitCandy.Web.Extensions;

public static class MetadataExtension
{
    const Int32 ShaBytesLength = 20;
    const String HexValuesInUppercase = "0123456789ABCDEF";
    const String HexValuesInLowercase = "0123456789abcdef";

    public static Byte[] AggregateSha(this Byte[] one, params Byte[][] twos)
    {
        Contract.Requires(one == null);
        Contract.Requires(one.Length == ShaBytesLength);

        var val = one.ToArray();
        foreach (var two in twos)
        {
            Contract.Assert(two != null && two.Length == ShaBytesLength);
            for (var i = 0; i < ShaBytesLength; i++)
                val[i] ^= two[i];
        }

        return val;
    }

    public static String BytesToString(this Byte[] bytes)
    {
        Contract.Requires(bytes != null);

        var length = bytes.Length;
        var chars = new Char[length * 2];
        for (Int32 i = 0, index = 0; i < length; i++)
        {
            var b = bytes[i];
            chars[index++] = HexValuesInLowercase[b & 0xf];
            chars[index++] = HexValuesInLowercase[b >> 4];
        }

        return new String(chars);
    }

    public static String ToFlagString(this Boolean flag, String trueStr, String falseStr) => flag ? trueStr : falseStr;

    public static Dictionary<String, Object> CastToDictionary(this Object values)
    {
        if (values == null) return null;

        var dictionary = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
        var properties = TypeDescriptor.GetProperties(values);
        foreach (PropertyDescriptor propertyDescriptor in properties)
        {
            var value = propertyDescriptor.GetValue(values);
            dictionary.Add(propertyDescriptor.Name, value);
        }
        return dictionary;
    }

    public static String ToShortSha(this String sha)
    {
        if (sha == null)
            return null;

        return sha.Length > 7 && sha.All(c => '0' <= c && c <= '9' || 'a' <= c && c <= 'f' || 'A' <= c && c <= 'F')
            ? sha[..7]
            : sha;
    }

    public static IEnumerable<SelectListItem> ToSelectListItem(this IEnumerable<String> items, String selected)
    {
        return items.Select(s => new SelectListItem
        {
            Text = s,
            Selected = s == selected,
        });
    }

    public static String CalcSha(this String str)
    {
        var sha = SHA1.Create();
        var data = Encoding.UTF8.GetBytes(str);
        data = sha.ComputeHash(data);
        return data.BytesToString();
    }

    public static String RepetitionIfEmpty(this String str, String repetition)
    {
        return String.IsNullOrWhiteSpace(str)
            ? repetition
            : str;
    }

    public static String ShortString(this String str, Int32 length)
    {
        if (str.IsNullOrEmpty()) return str;

        var wide = 0;
        var len = 0;
        foreach (var ch in str)
        {
            // simple place a wide character
            wide += ch < 0x1000 ? 1 : 2;
            len++;
            if (wide > length)
                return str[..(len - 4)] + " ...";
        }
        return str;
    }

    public static Byte[] ToBytes(this Stream stream)
    {
        if (stream == null)
            return null;

        if (stream is MemoryStream)
        {
            var ms = (MemoryStream)stream;
            return ms.ToArray();
        }

        var buffer = new Byte[16 * 1024];
        using (var ms = new MemoryStream())
        {
            Int32 len;
            while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, len);
            return ms.ToArray();
        }
    }

    public static String ReadLines(this StringReader reader, Int32 lineCount)
    {
        var sb = new StringBuilder();
        while (lineCount-- > 0)
        {
            sb.Append(reader.ReadLine());
            if (lineCount > 0)
                sb.AppendLine();
        }
        return sb.ToString();
    }

    public static String ToLocateString(this ChangeKind changeKind)
    {
        switch (changeKind)
        {
            case ChangeKind.Added:
                return SR.Repository_FileAdded;
            case ChangeKind.Copied:
                return SR.Repository_FileCopied;
            case ChangeKind.Deleted:
                return SR.Repository_FileDeleted;
            case ChangeKind.Ignored:
                return SR.Repository_FileIgnored;
            case ChangeKind.Modified:
                return SR.Repository_FileModified;
            case ChangeKind.Renamed:
                return SR.Repository_FileRenamed;
            case ChangeKind.TypeChanged:
                return SR.Repository_FileTypeChanged;
            case ChangeKind.Unmodified:
                return SR.Repository_FileUnmodified;
            case ChangeKind.Untracked:
                return SR.Repository_FileUntracked;
            default:
                return String.Empty;
        }
    }

    [Pure]
    public static String SafyToString(this Object obj)
    {
        if (obj == null)
            return null;
        return obj.ToString();
    }

    public static String GetUrl(this TreeModel model, String action, String path)
    {
        //var root = HttpRuntime.AppDomainAppVirtualPath;
        //root = root.EnsureEnd("/");
        var root = "/";

        return root + $"{model.Owner}/{model.Name}/{action}/{model.ReferenceName ?? model.Commit.Sha}/{path}";
    }

    //public static String GetUrl(this PathBarModel model, String action, String path)
    //{
    //    return $"/{model.Owner}/{model.Name}/{action}/{model.ReferenceName ?? model.ReferenceSha}/{path}";
    //}

    public static void FixMarkdown(this TreeEntryModel model, String baseurl)
    {
        var txt = model.TextContent;

        // 预处理Markdown链接和图片
        var p = 0;
        var ps = new Int32[] { 0, 0 };
        while (true)
        {
            var url = txt.Substring("](", ")", p, ps);
            if (url.IsNullOrEmpty() || ps[0] < 0) break;

            if (!url.IsNullOrEmpty() && !url.StartsWithIgnoreCase("http://", "https://", "/"))
            {
                // 处理Url。找到当前文件路径
                var path = model.Path;
                var p2 = path.LastIndexOf("/");
                if (p2 >= 0)
                    path = path[..(p2 + 1)];
                else
                    path = "";

                var name = model.ReferenceName;
                if (name.IsNullOrEmpty()) name = "master";
                url = $"{baseurl}/Blob/{name}/{path}{url}";

                // 重新拼接
                txt = txt[..ps[0]] + url + txt[ps[1]..];
            }

            // 下一次
            p = ps[0];
        }

        model.TextContent = txt;
    }
}