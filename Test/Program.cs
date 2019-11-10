using System;
using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;
using NewLife.Log;

namespace Test
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {
                Test1();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK");
            Console.ReadLine();
        }

        static void Test1()
        {
            //var remoteUrl = "https://gitee.com/NewLifeX/NewLife.Cube";
            var remoteUrl = "https://gitee.com/NewLifeX/GitCandy";
            var xx = "xx".GetFullPath();
            //if (Directory.Exists(xx)) Directory.Delete(xx, true);
            var p = xx;
            if (!Directory.Exists(xx)) p = Repository.Init(xx, true);
            using (var repo = new Repository(p))
            {
                repo.Network.Remotes.Add("origin", remoteUrl, "+refs/*:refs/*");

                //var refs = repo.Network.ListReferences("origin").ToList();
                //XTrace.WriteLine("发现分支：{0}", refs.Select(e => e.TargetIdentifier));

                //repo.Network.Fetch("origin", new[] { "master" });
                repo.Network.Fetch("origin", new string[0]);
            }
        }
    }
}
