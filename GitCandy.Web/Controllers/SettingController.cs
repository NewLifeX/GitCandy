using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using GitCandy.Configuration;
using GitCandy.Filters;
using GitCandy.Models;
using GitCandy.Ssh;
using NewLife.Log;

namespace GitCandy.Controllers
{
    [Administrator]
    public class SettingController : CandyControllerBase
    {
        public ActionResult Edit()
        {
            var config = UserConfiguration.Current;
            var model = new SettingModel
            {
                IsPublicServer = config.IsPublicServer,
                ForceSsl = config.ForceSsl,
                SslPort = config.SslPort,
                SshPort = config.SshPort,
                EnableSsh = config.EnableSsh,
                LocalSkipCustomError = config.LocalSkipCustomError,
                AllowRegisterUser = config.AllowRegisterUser,
                AllowRepositoryCreation = config.AllowRepositoryCreation,
                RepositoryPath = config.RepositoryPath.GetFullPath(),
                CachePath = config.CachePath.GetFullPath(),
                GitCorePath = config.GitCorePath.GetFullPath(),
                NumberOfCommitsPerPage = config.Commits,
                NumberOfItemsPerList = config.PageSize,
                NumberOfRepositoryContributors = config.Contributors,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SettingModel model)
        {
            var needRestart = false;
            var needRestartSshServer = false;

            if (ModelState.IsValid)
            {
                var config = UserConfiguration.Current;

                needRestart = (config.CachePath != model.CachePath);
                needRestartSshServer = config.SshPort != model.SshPort || config.EnableSsh != model.EnableSsh;

                config.IsPublicServer = model.IsPublicServer;
                config.ForceSsl = model.ForceSsl;
                config.SslPort = model.SslPort;
                config.SshPort = model.SshPort;
                config.EnableSsh = model.EnableSsh;
                config.LocalSkipCustomError = model.LocalSkipCustomError;
                config.AllowRegisterUser = model.AllowRegisterUser;
                config.AllowRepositoryCreation = model.AllowRepositoryCreation;
                config.RepositoryPath = model.RepositoryPath;
                config.CachePath = model.CachePath;
                config.GitCorePath = model.GitCorePath;
                config.Commits = model.NumberOfCommitsPerPage;
                config.PageSize = model.NumberOfItemsPerList;
                config.Contributors = model.NumberOfRepositoryContributors;
                config.Save();
                ModelState.Clear();
            }

            XTrace.WriteLine("Settings updated by {0}#{1}", Token.Username, Token.UserID);

            if (needRestart)
            {
                SshServerConfig.StopSshServer();
                HttpRuntime.UnloadAppDomain();
            }
            else if (needRestartSshServer)
            {
                SshServerConfig.RestartSshServer();
            }

            return View(model);
        }

        public ActionResult Restart(String conform)
        {
            if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
            {
                HttpRuntime.UnloadAppDomain();
                return RedirectToStartPage();
            }
            return View();
        }

        public ActionResult ReGenSsh(String conform)
        {
            if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
            {
                UserConfiguration.Current.HostKeys.Clear();
                foreach (var type in KeyUtils.SupportedAlgorithms)
                {
                    UserConfiguration.Current.HostKeys.Add(new HostKey { KeyType = type, KeyXml = KeyUtils.GeneratePrivateKey(type) });
                }
                UserConfiguration.Current.Save();

                SshServerConfig.RestartSshServer();

                return RedirectToAction("Edit");
            }
            return View();
        }
    }
}
