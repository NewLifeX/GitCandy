using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NewLife.Xml;

namespace GitCandy.Configuration
{
    [XmlConfigFile("Config\\Git.Config", 15000)]
    public class UserConfiguration : XmlConfig<UserConfiguration>
    {
        #region 属性
        public bool IsPublicServer { get; set; } = true;

        public bool ForceSsl { get; set; }

        public int SslPort { get; set; } = 443;

        public bool LocalSkipCustomError { get; set; }

        public bool AllowRegisterUser { get; set; } = true;

        public bool AllowRepositoryCreation { get; set; } = true;

        public String RepositoryPath { get; set; } = "..\\Repos";

        public String CachePath { get; set; } = "..\\Cache";

        public String GitCorePath { get; set; }

        public int Commits { get; set; } = 30;

        public int PageSize { get; set; } = 30;

        public int Contributors { get; set; } = 50;

        public int SshPort { get; set; } = 22;

        public bool EnableSsh { get; set; } = true;

        public List<HostKey> HostKeys { get; set; } = new List<HostKey>();
        #endregion

        public UserConfiguration()
        {
        }

        protected override void OnNew()
        {
            GitCorePath = GetGitCore();

            base.OnNew();
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
}