using System.Globalization;
using System.IO.Compression;
using GitCandy.Data;
using GitCandy.Filters;
using GitCandy.Git;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using NewLife.Log;

namespace GitCandy.Web.Controllers;

public class GitController : CandyControllerBase
{
    public RepositoryService RepositoryService { get; set; } = new RepositoryService();

    [SmartGit]
    public async Task<ActionResult> Smart(String owner, String project, String service, String verb)
    {
        return verb switch
        {
            "info/refs" => await InfoRefs(owner, project, service),
            "git-upload-pack" => ExecutePack(owner, project, "git-upload-pack"),
            "git-receive-pack" => ExecutePack(owner, project, "git-receive-pack"),
            _ => RedirectToAction("Tree", "Repository", new { Owner = owner, Name = project }),
        };
    }

    protected async Task<ActionResult> InfoRefs(String owner, String project, String service)
    {
        //Response.Charset = "";
        Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-advertisement", service);

        SetNoCache();

        await Response.WriteAsync(FormatMessage(String.Format(CultureInfo.InvariantCulture, "# service={0}\n", service)));
        await Response.WriteAsync(FlushMessage());

        try
        {
            using (var git = new GitService(owner, project))
            {
                var svc = service[4..];
                git.InfoRefs(svc, GetInputStream(), Response.Body);
            }
            return new EmptyResult();
        }
        catch (RepositoryNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    protected ActionResult ExecutePack(String owner, String project, String service)
    {
        //Response.Charset = "";
        Response.ContentType = String.Format(CultureInfo.InvariantCulture, "application/x-{0}-result", service);
        SetNoCache();

        try
        {
            using (var git = new GitService(owner, project))
            {
                git.Log = XTrace.Log;
                var svc = service[4..];
                git.ExecutePack(svc, GetInputStream(), Response.Body);

                // 拦截提交
                if (svc == "receive-pack") Task.Run(() => UpdateRepo(owner, project));
            }
            return new EmptyResult();
        }
        catch (RepositoryNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>更新仓库统计信息</summary>
    /// <param name="project"></param>
    private void UpdateRepo(String owner, String project)
    {
        using var git = new GitService(owner, project);
        var cms = 0;
        var cts = 0;

        // 修正提交数、分支、参与人等
        var commit = git.Repository.Head.Tip;
        if (commit != null)
        {
            var ancestors = git.Repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit });

            var set = new HashSet<String>();
            foreach (var ancestor in ancestors)
            {
                cms++;
                if (set.Add(ancestor.Author.ToString()))
                    cts++;
            }
        }

        var repo = NewLife.GitCandy.Entity.Repository.FindByOwnerAndName(owner, project);
        if (repo != null)
        {
            if (cms > 0) repo.Commits = cms;
            repo.Branches = git.Repository.Branches.Count();
            if (cts > 0) repo.Contributors = cts;
            repo.Files = FilesInCommit(commit, out var size);
            repo.Size = size;
            repo.LastCommit = commit.Committer.When.LocalDateTime;

            repo.SaveAsync();
        }
    }

    private Int32 FilesInCommit(Commit commit, out Int64 sourceSize)
    {
        var count = 0;
        var stack = new Stack<Tree>();
        sourceSize = 0;

        var repo = ((IBelongToARepository)commit).Repository;

        stack.Push(commit.Tree);
        while (stack.Count != 0)
        {
            var tree = stack.Pop();
            foreach (var entry in tree)
                switch (entry.TargetType)
                {
                    case TreeEntryTargetType.Blob:
                        count++;
                        sourceSize += repo.ObjectDatabase.RetrieveObjectMetadata(entry.Target.Id).Size;
                        break;
                    case TreeEntryTargetType.Tree:
                        stack.Push((Tree)entry.Target);
                        break;
                }
        }
        return count;
    }

    #region GitController Extension
    private void SetNoCache()
    {
        //Response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
        //Response.AddHeader("Pragma", "no-cache");
        //Response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");

        Response.Headers.Expires = "Fri, 01 Jan 1980 00:00:00 GMT";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.CacheControl = "no-cache, max-age=0, must-revalidate";
    }

    private Stream GetInputStream()
    {
        if (Request.Headers["Content-Encoding"] == "gzip")
            return new GZipStream(Request.Body, CompressionMode.Decompress);

        return Request.Body;
    }

    private static String FormatMessage(String input) => (input.Length + 4).ToString("X4", CultureInfo.InvariantCulture) + input;

    private static String FlushMessage() => "0000";
    #endregion
}