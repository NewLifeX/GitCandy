﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GitCandy.Base;

/// <summary>文件帮助类</summary>
public static class FileHelper
{
    public const String BinaryMimeType = "application/octet-stream";

    public static readonly IReadOnlyDictionary<String, String> BrushMapping;
    //public static readonly IReadOnlyDictionary<String, String> AchiveMapping;
    public static readonly IReadOnlyCollection<String> ImageSet;

    private static readonly String[] SizeUnits = { "B", "KiB", "MiB", "GiB", "TiB" };
    private static readonly Encoding[] BomMapping =
    {
        new UTF8Encoding(true),             // UTF-8
        new UnicodeEncoding(false, true),   // UTF-16 (LE)
        new UnicodeEncoding(true, true),    // UTF-16 (BE)
        new UTF32Encoding(false, true),     // UTF-32 (LE)
        new UTF32Encoding(true, true),      // UTF-32 (BE)
    };
    private static readonly Byte[][] Boms;

    static FileHelper()
    {
        BrushMapping = new ReadOnlyDictionary<String, String>(new Dictionary<String, String>
        {
            { ".sh", "hash" },
            { ".cs", "cs" },
            { ".h", "cpp" },
            { ".hh", "cpp" },
            { ".c", "cpp" },
            { ".cc", "cpp" },
            { ".cp", "cpp" },
            { ".cpp", "cpp" },
            { ".c++", "cpp" },
            { ".cxx", "cpp" },
            { ".css", "css" },
            { ".ini", "ini" },
            { ".json", "json" },
            { ".java", "java" },
            { ".js", "javascript" },
            { ".jscript", "javascript" },
            { ".javascript", "javascript" },
            { ".php", "php" },
            { ".pl", "perl" },
            { ".py", "python" },
            { ".rb", "ruby" },
            { ".sql", "sql" },
            { ".as", "actionscript" },
            { ".applescript", "applescript" },
            { ".bf", "brainfuck" },
            { ".cmake", "cmake" },
            { ".clj", "clojure" },
            { ".coffee", "coffeescript" },
            { ".bat", "dos" },
            { ".cmd", "dos" },
            { ".pas", "delphi" },
            { ".erl", "erlang" },
            { ".fs", "fsharp" },
            { ".fsx", "fsharp" },
            { ".haml", "haml" },
            { ".go", "go" },
            { ".hs", "haskell" },
            { ".lisp", "lisp" },
            { ".lsp", "lisp" },
            { ".cl", "lisp" },
            { ".lua", "lua" },
            { ".md", "markdown" },
            { ".m", "matlab" },
            { ".rs", "rust" },
            { ".scala", "scala" },
            { ".vb", "vbnet" },
            { ".vbs", "vbscript" },

            { ".xml", "xml" },
            { ".htm", "xml" },
            { ".html", "xml" },
            { ".shtml", "xml" },
            { ".webp", "xml" },
            { ".xht", "xml" },
            { ".xhtml", "xml" },
            { ".config", "xml" },
            { ".vssettings", "xml" },
            { ".csproj", "xml" },
            { ".vbproj", "xml" },
            { ".resx", "xml" },
            { ".xaml", "xml" },
            { ".vsmdi", "xml" },
            { ".testsettings", "xml" },

            // Thanks for st52 <130990851@qq.com> list the extensions of action script
            { ".old", "xml" },
            { ".as3proj", "xml" },
            { ".actionscriptproperties", "xml" },
            { ".project", "xml" },
            { ".morn", "xml" },

            { ".diff", "diff" },
            { ".patch", "diff" },
            { ".http", "http" },
        });

        //AchiveMapping = new ReadOnlyDictionary<String, String>(new Dictionary<String, String>
        //{
        //    { "zip", "application/zip" },
        //    { "gz", "application/x-gzip" },
        //    { "tar.gz", "application/x-tgz" },
        //});

        ImageSet = new ReadOnlyCollection<String>(new List<String>
        {
            ".bmp",
            ".gif",
            ".jpeg", ".jpg", ".jpe",
            ".png",
            ".svg", ".svgz",
            ".tiff", ".tif",
            ".ico",
            ".pbm",
        });

        Boms = BomMapping.Select(s => s.GetPreamble()).ToArray();
    }

    public static String GetBrush(String path)
    {
        var extension = Path.GetExtension(path).ToLower();
        if (BrushMapping.ContainsKey(extension))
            return BrushMapping[extension];
        return "no-highlight";
    }

    public static Encoding DetectEncoding(Byte[] bytes, params Encoding[] encodings)
    {
        var index = BomIndex(bytes);
        if (index != -1) // Read BOM if existing
            return BomMapping[index];

        if (bytes.Length > 1024)
            bytes = bytes.Take(1024).ToArray();

        var collection = encodings.Concat(new[]
                {
                    // generic
                    Encoding.UTF8,
                    // default encoding for user selected UI lanaguage
                    Encoding.GetEncoding(CultureInfo.CurrentUICulture.TextInfo.ANSICodePage),
                    // as more as possible, default encoding for server side
                    Encoding.Default,
                })
            .Where(s => s != null)
            .GroupBy(s => s.CodePage)
            .Select(s => s.First())
            .ToArray();

        var hasNonSingleByte = collection.Any(s => !s.IsSingleByte);
        if (hasNonSingleByte)
        {
            var pendings = collection
                .Where(s => IsMatchEncoding(s, bytes))
                .ToArray();

            return pendings.FirstOrDefault(s => !s.IsSingleByte)
                ?? pendings.FirstOrDefault();
        }
        else
        {
            return collection.FirstOrDefault(s => IsMatchEncoding(s, bytes));
        }
    }

    public static String ReadToEnd(Byte[] bytes, Encoding encoding = null, String newline = null)
    {
        using (var reader = new StreamReader(new MemoryStream(bytes), encoding ?? Encoding.UTF8, true))
        {
            var str = reader.ReadToEnd();
            return RegularExpression.ReplaceNewline.Replace(str, newline ?? Environment.NewLine);
        }
    }

    public static String GetSizeString(Int64 size)
    {
        if (size < 0)
            return "unknow size";

        Double r = size;
        foreach (var unit in SizeUnits)
        {
            if (r < 1000)
                return r.ToString("f2") + " " + unit;
            r /= 1024;
        }

        return "largest size";
    }

    public static Byte[] ReplaceNewline(Byte[] bytes, Encoding encoding = null, String newline = null)
    {
        if (newline == null)
            return bytes;
        encoding ??= Encoding.UTF8;

        var bomIndex = BomIndex(bytes);
        var pure = encoding.GetBytes(ReadToEnd(bytes, encoding, newline));
        if (bomIndex != -1)
        {
            var bom = Boms[bomIndex];
            var buffer = new Byte[bom.Length + pure.Length];
            Array.Copy(bom, buffer, bom.Length);
            Array.Copy(pure, 0, buffer, bom.Length, pure.Length);
            return buffer;
        }

        return pure;
    }

    private static Int32 BomIndex(Byte[] bytes)
    {
        for (var i = 0; i < Boms.Length; i++)
        {
            if (bytes.Length < Boms[i].Length)
                continue;
            var flag = true;
            for (var j = 0; flag && j < Boms[i].Length; j++)
                flag = bytes[j] == Boms[i][j];
            if (flag)
                return i;
        }
        return -1;
    }

    private static Boolean IsMatchEncoding(Encoding encoding, Byte[] bytes)
    {
        try
        {
            var s = encoding.GetString(bytes); // ignore unknow BOM, supposing there is no BOM
            var r = encoding.GetBytes(s);

            var match = 0.0;
            for (var i = 0; i < r.Length; i++)
                if (bytes[i] == r[i])
                    match++;
            if (match >= r.Length * 0.9)
                return true;
        }
        catch { }
        return false;
    }

    //public static String ReadMarkdown(Byte[] bytes, Encoding encoding, String baseurl, String path)
    //{
    //    var txt = bytes.ToStr(encoding);

    //    // 预处理Markdown链接和图片
    //    var p = 0;
    //    var ps = new Int32[] { 0, 0 };
    //    while (true)
    //    {
    //        var url = txt.Substring("](", ")", p, ps);
    //        if (url.IsNullOrEmpty() || ps[0] < 0) break;

    //        if (!url.IsNullOrEmpty() && !url.StartsWithIgnoreCase("http://", "https://", "/"))
    //        {
    //            // 处理Url。找到当前文件路径

    //            // 重新拼接
    //            txt = txt.Substring(0, ps[0]) + url + txt.Substring(ps[1]);
    //        }

    //        // 下一次
    //        p = ps[0];
    //    }

    //    return txt;
    //}
}