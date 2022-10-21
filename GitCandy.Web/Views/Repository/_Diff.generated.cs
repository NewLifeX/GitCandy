﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using GitCandy;
    using GitCandy.Base;
    using GitCandy.Configuration;
    using GitCandy.Extensions;
    using GitCandy.Models;
    using GitCandy.Web;
    using GitCandy.Web.App_GlobalResources;
    using NewLife;
    using NewLife.Cube;
    using NewLife.Reflection;
    using NewLife.Serialization;
    using NewLife.Web;
    using XCode;
    using XCode.Membership;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Repository/_Diff.cshtml")]
    public partial class _Views_Repository__Diff_cshtml : System.Web.Mvc.WebViewPage<GitCandy.Models.CommitModel>
    {
        public _Views_Repository__Diff_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<div");

WriteLiteral(" class=\"page-divider\"");

WriteLiteral("></div>\r\n<div>");

            
            #line 4 "..\..\Views\Repository\_Diff.cshtml"
Write(String.Format(SR.Repository_ChangedSummary, Model.Changes.Count(), Model.Changes.Sum(s => s.LinesAdded), Model.Changes.Sum(s => s.LinesDeleted)));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

            
            #line 5 "..\..\Views\Repository\_Diff.cshtml"
 foreach (var change in Model.Changes)
{
    var removed = change.ChangeKind == LibGit2Sharp.ChangeKind.Deleted || change.ChangeKind == LibGit2Sharp.ChangeKind.Ignored;

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n        <div>\r\n            <div");

WriteLiteral(" class=\"col-md-2 pull-left\"");

WriteLiteral(">");

            
            #line 10 "..\..\Views\Repository\_Diff.cshtml"
                                       Write(change.ChangeKind.ToLocateString());

            
            #line default
            #line hidden
WriteLiteral(" +");

            
            #line 10 "..\..\Views\Repository\_Diff.cshtml"
                                                                            Write(change.LinesAdded);

            
            #line default
            #line hidden
WriteLiteral(" -");

            
            #line 10 "..\..\Views\Repository\_Diff.cshtml"
                                                                                                Write(change.LinesDeleted);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n            <div");

WriteLiteral(" class=\"col-md-10 pull-right\"");

WriteLiteral(">\r\n");

            
            #line 12 "..\..\Views\Repository\_Diff.cshtml"
                
            
            #line default
            #line hidden
            
            #line 12 "..\..\Views\Repository\_Diff.cshtml"
                 if (change.OldPath != change.Path)
                {
                    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Repository\_Diff.cshtml"
               Write(change.OldPath);

            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Repository\_Diff.cshtml"
                                    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Repository\_Diff.cshtml"
                               Write(Html.Raw(" → "));

            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Repository\_Diff.cshtml"
                                                    
                }

            
            #line default
            #line hidden
WriteLiteral("                ");

            
            #line 16 "..\..\Views\Repository\_Diff.cshtml"
            Write(removed
                    ? Html.Raw(change.Path)
                    : Html.ActionLink(change.Path, "Blob", Html.OverRoute(new { path = (Model.ReferenceName ?? Model.Sha) + "/" + change.Path })));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 22 "..\..\Views\Repository\_Diff.cshtml"
}

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\Repository\_Diff.cshtml"
 foreach (var change in Model.Changes)
{
    var removed = change.ChangeKind == LibGit2Sharp.ChangeKind.Deleted || change.ChangeKind == LibGit2Sharp.ChangeKind.Ignored;

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"page-divider\"");

WriteLiteral("></div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n        <div>\r\n            <div");

WriteLiteral(" class=\"col-md-2\"");

WriteLiteral(">");

            
            #line 29 "..\..\Views\Repository\_Diff.cshtml"
                             Write(change.ChangeKind.ToLocateString());

            
            #line default
            #line hidden
WriteLiteral(" +");

            
            #line 29 "..\..\Views\Repository\_Diff.cshtml"
                                                                  Write(change.LinesAdded);

            
            #line default
            #line hidden
WriteLiteral(" -");

            
            #line 29 "..\..\Views\Repository\_Diff.cshtml"
                                                                                      Write(change.LinesDeleted);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n            <div");

WriteLiteral(" class=\"col-md-10\"");

WriteLiteral(">\r\n");

            
            #line 31 "..\..\Views\Repository\_Diff.cshtml"
                
            
            #line default
            #line hidden
            
            #line 31 "..\..\Views\Repository\_Diff.cshtml"
                 if (change.OldPath != change.Path)
                {
                    
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Repository\_Diff.cshtml"
               Write(change.OldPath);

            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Repository\_Diff.cshtml"
                                    
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Repository\_Diff.cshtml"
                               Write(Html.Raw(" → "));

            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Repository\_Diff.cshtml"
                                                    
                }

            
            #line default
            #line hidden
WriteLiteral("                ");

            
            #line 35 "..\..\Views\Repository\_Diff.cshtml"
            Write(removed
                    ? Html.Raw(change.Path)
                    : Html.ActionLink(change.Path, "Blob", Html.OverRoute(new { path = (Model.ReferenceName ?? Model.Sha) + "/" + change.Path })));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 40 "..\..\Views\Repository\_Diff.cshtml"
        
            
            #line default
            #line hidden
            
            #line 40 "..\..\Views\Repository\_Diff.cshtml"
         if (!removed)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"col-md-12\"");

WriteLiteral(">\r\n                <pre><code");

WriteLiteral(" class=\"language diff\"");

WriteLiteral(">");

            
            #line 43 "..\..\Views\Repository\_Diff.cshtml"
                                            Write(change.Patch);

            
            #line default
            #line hidden
WriteLiteral("</code></pre>\r\n            </div>\r\n");

            
            #line 45 "..\..\Views\Repository\_Diff.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

            
            #line 47 "..\..\Views\Repository\_Diff.cshtml"
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
