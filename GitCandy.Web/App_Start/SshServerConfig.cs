using System;
using System.Net;
using System.Threading.Tasks;
using GitCandy.Configuration;
using GitCandy.Git;
using GitCandy.Ssh;
using NewLife.Log;

namespace GitCandy
{
    public static class SshServerConfig
    {
        private static SshServer _server = null;

        public static void StartSshServer()
        {
            if (!UserConfiguration.Current.EnableSsh || _server != null)
                return;

            _server = new SshServer(new StartingInfo(IPAddress.IPv6Any, UserConfiguration.Current.SshPort));
            _server.ConnectionAccepted += (s, e) => new GitSshService(e);
            _server.ExceptionRasied += (s, e) =>
            {
                if (!(e is SshConnectionException))
                    XTrace.Log.Error(e.ToString());
            };
            foreach (var key in UserConfiguration.Current.HostKeys)
            {
                _server.AddHostKey(key.KeyType, key.KeyXml);
            }
            for (var i = 1; i <= 10; i++)
            {
                try
                {
                    _server.Start();
                    XTrace.WriteLine("SSH server started.");
                    break;
                }
                catch (Exception ex)
                {
                    XTrace.WriteLine("Attempt to start SSH server failed in {0} times. {1}", i, ex);
                    Task.Delay(1000).Wait();
                }
            }
        }

        public static void StopSshServer()
        {
            if (_server == null)
                return;

            _server.Stop();
            _server = null;

            XTrace.WriteLine("SSH server stoped.");
        }

        public static void RestartSshServer()
        {
            StopSshServer();
            StartSshServer();
        }
    }
}
