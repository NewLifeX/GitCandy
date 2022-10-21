using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NewLife;
using NewLife.Configuration;

namespace GitCandy.Configuration;

[Config("Config\\Git.Config")]
public class UserConfiguration : Config<UserConfiguration>
{
    #region 属性
    /// <summary>开放服务</summary>
    [DisplayName("开放服务")]
    public Boolean IsPublicServer { get; set; } = true;

    ///// <summary>强制SSL</summary>
    //[DisplayName("强制SSL")]
    //public Boolean ForceSsl { get; set; }

    ///// <summary>SSL端口</summary>
    //[DisplayName("SSL端口")]
    //public Int32 SslPort { get; set; } = 443;

    ///// <summary>开启SSH</summary>
    //[DisplayName("开启SSH")]
    //public bool EnableSsh { get; set; } = true;

    ///// <summary>SSH端口</summary>
    //[DisplayName("SSH端口")]
    //public int SshPort { get; set; } = 22;

    ///// <summary>跳过本地错误</summary>
    //[DisplayName("跳过本地错误")]
    //public Boolean LocalSkipCustomError { get; set; }

    /// <summary>允许注册</summary>
    [DisplayName("允许注册")]
    public Boolean AllowRegisterUser { get; set; } = true;

    /// <summary>允许创建代码库</summary>
    [DisplayName("允许创建代码库")]
    public Boolean AllowRepositoryCreation { get; set; } = true;

    /// <summary>代码库存储路径</summary>
    [DisplayName("代码库存储路径")]
    public String RepositoryPath { get; set; } = "..\\Repos";

    /// <summary>缓存路径</summary>
    [DisplayName("缓存路径")]
    public String CachePath { get; set; } = "..\\Cache";

    /// <summary>Git-Core路径</summary>
    [DisplayName("Git-Core路径")]
    public String GitCorePath { get; set; }

    /// <summary>每页提交数</summary>
    [DisplayName("每页提交数")]
    public Int32 Commits { get; set; } = 30;

    /// <summary>分页大小</summary>
    [DisplayName("分页大小")]
    public Int32 PageSize { get; set; } = 30;

    /// <summary>显示参与者数</summary>
    [DisplayName("显示参与者数")]
    public Int32 Contributors { get; set; } = 50;

    /// <summary>允许打包。默认true</summary>
    [DisplayName("允许打包。默认true")]
    public Boolean AllowArchive { get; set; } = true;

    /// <summary>允许查看审阅。默认true</summary>
    [DisplayName("允许查看审阅。默认true")]
    public Boolean AllowBlame { get; set; } = true;

    /// <summary>允许查看提交。默认true</summary>
    [DisplayName("允许查看提交。默认true")]
    public Boolean AllowCommits { get; set; } = true;

    /// <summary>允许查看贡献者。默认true</summary>
    [DisplayName("允许查看贡献者。默认true")]
    public Boolean AllowContributors { get; set; } = true;

    /// <summary>允许查看分支差异。默认true</summary>
    [DisplayName("允许查看分支差异。默认true")]
    public Boolean AllowHistoryDivergence { get; set; } = true;

    /// <summary>允许查看摘要。默认true</summary>
    [DisplayName("允许查看摘要。默认true")]
    public Boolean AllowSummary { get; set; } = true;
    #endregion

    protected override void OnLoaded()
    {
        if (GitCorePath.IsNullOrEmpty()) GitCorePath = GetGitCore();

        base.OnLoaded();
    }

    private String GetGitCore()
    {
        var list = new List<String>();
        var variable = Environment.GetEnvironmentVariable("path");
        if (variable != null)
            list.AddRange(variable.Split(';'));

        list.Add(Environment.GetEnvironmentVariable("ProgramW6432"));
        list.Add(Environment.GetEnvironmentVariable("ProgramFiles"));

        foreach (var drive in Environment.GetLogicalDrives())
        {
            list.Add(drive + @"Program Files\Git");
            list.Add(drive + @"Program Files (x86)\Git");
            list.Add(drive + @"Program Files\PortableGit");
            list.Add(drive + @"Program Files (x86)\PortableGit");
            list.Add(drive + @"PortableGit");
        }

        list = list.Where(x => !String.IsNullOrEmpty(x)).Distinct().ToList();
        foreach (var path in list)
        {
            var ret = SearchPath(path);
            if (ret != null)
                return ret;
        }

        return "";
    }

    private String SearchPath(String path)
    {
        var patterns = new[] {
            @"..\libexec\git-core", // git 1.x
            @"libexec\git-core", // git 1.x
            @"..\mingw64\libexec\git-core", // git 2.x
            @"mingw64\libexec\git-core", // git 2.x
        };
        foreach (var pattern in patterns)
        {
            var fullpath = new DirectoryInfo(Path.Combine(path, pattern)).FullName;
            if (File.Exists(Path.Combine(fullpath, "git.exe"))
                && File.Exists(Path.Combine(fullpath, "git-receive-pack.exe"))
                && File.Exists(Path.Combine(fullpath, "git-upload-archive.exe"))
                && File.Exists(Path.Combine(fullpath, "git-upload-pack.exe")))
                return fullpath;
        }
        return null;
    }
}