﻿using LibGit2Sharp;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NewLife.GitCandy.Entity;

namespace GitCandy.Web.Extensions;

public static class CommitLogExtension
{
    public static IEnumerable<Commit> PathFilter(this IEnumerable<Commit> log, String path)
    {
        if (String.IsNullOrEmpty(path))
            return log;

        return log.Where(s =>
        {
            var pathEntry = s[path];
            var parent = s.Parents.FirstOrDefault();
            if (parent == null)
                return pathEntry != null;

            var parentPathEntry = parent[path];
            if (pathEntry == null && parentPathEntry == null)
                return false;
            if (pathEntry != null && parentPathEntry != null)
                return pathEntry.Target.Sha != parentPathEntry.Target.Sha;
            return true;
        });
    }

    public static IHtmlContent Link(this HtmlHelper html, Signature sign)
    {
        var user = User.FindByName(sign.Name) ?? User.FindByEmail(sign.Email);
        if (user != null)
            return html.ActionLink(user + "", "Detail", "Account", new { name = user.Name }, new { target = "_blank" });
        else
            return new HtmlString(sign.Name);
    }
}