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
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using GitCandy;
    using GitCandy.App_GlobalResources;
    using GitCandy.Base;
    using GitCandy.Configuration;
    using GitCandy.Extensions;
    using GitCandy.Models;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Layout.cshtml")]
    public partial class _Views_Shared__Layout_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_Shared__Layout_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 1 "..\..\Views\Shared\_Layout.cshtml"
  
    var token = GitCandy.Security.Token.Current;

            
            #line default
            #line hidden
WriteLiteral("\r\n<!DOCTYPE html>\r\n<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteAttribute("lang", Tuple.Create(" lang=\"", 116), Tuple.Create("\"", 136)
            
            #line 5 "..\..\Views\Shared\_Layout.cshtml"
, Tuple.Create(Tuple.Create("", 123), Tuple.Create<System.Object, System.Int32>(ViewBag.Lang
            
            #line default
            #line hidden
, 123), false)
);

WriteLiteral(">\r\n<head>\r\n    <meta");

WriteLiteral(" charset=\"utf-8\"");

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" name=\"viewport\"");

WriteLiteral(" content=\"width=device-width, initial-scale=1.0\"");

WriteLiteral(" />\r\n    <title>");

            
            #line 9 "..\..\Views\Shared\_Layout.cshtml"
      Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n");

WriteLiteral("    ");

            
            #line 10 "..\..\Views\Shared\_Layout.cshtml"
Write(Styles.Render("~/bundles/css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n</head>\r\n<body>\r\n    <div");

WriteLiteral(" class=\"navbar navbar-default\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"container\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"navbar-header\"");

WriteLiteral(">\r\n                <button");

WriteLiteral(" class=\"navbar-toggle\"");

WriteLiteral(" type=\"button\"");

WriteLiteral(" data-toggle=\"collapse\"");

WriteLiteral(" data-target=\".navbar-responsive-collapse\"");

WriteLiteral(">\r\n                    <span");

WriteLiteral(" class=\"sr-only\"");

WriteLiteral("></span>\r\n                    <span");

WriteLiteral(" class=\"icon-bar\"");

WriteLiteral("></span>\r\n                    <span");

WriteLiteral(" class=\"icon-bar\"");

WriteLiteral("></span>\r\n                    <span");

WriteLiteral(" class=\"icon-bar\"");

WriteLiteral("></span>\r\n                </button>\r\n                <a");

WriteLiteral(" class=\"navbar-brand\"");

WriteLiteral(" href=\"/\"");

WriteLiteral(">");

            
            #line 22 "..\..\Views\Shared\_Layout.cshtml"
                                            Write(SR.Shared_AppTitle);

            
            #line default
            #line hidden
WriteLiteral("<sub>Alpha</sub></a>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"navbar-collapse collapse navbar-responsive-collapse\"");

WriteLiteral(">\r\n                <ul");

WriteLiteral(" class=\"nav navbar-nav\"");

WriteLiteral(">\r\n                    <li>");

            
            #line 26 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.ActionLink(SR.Shared_Repositories, "Index", "Repository"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n                    <li>");

            
            #line 27 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.ActionLink(SR.Shared_About, "About", "Home"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 28 "..\..\Views\Shared\_Layout.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 28 "..\..\Views\Shared\_Layout.cshtml"
                     if (token != null && token.IsSystemAdministrator)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 30 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Users, "Index", "Account"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 31 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Teams, "Index", "Team"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 32 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Settings, "Edit", "Setting"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 33 "..\..\Views\Shared\_Layout.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </ul>\r\n                <ul");

WriteLiteral(" class=\"nav navbar-nav navbar-right\"");

WriteLiteral(">\r\n");

            
            #line 36 "..\..\Views\Shared\_Layout.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 36 "..\..\Views\Shared\_Layout.cshtml"
                     if (token == null)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 38 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Register, "Create", "Account"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 39 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Login, "Login", "Account", new { ReturnUrl = ViewContext.HttpContext.Request.Url.PathAndQuery }, null));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 40 "..\..\Views\Shared\_Layout.cshtml"
                    }
                    else
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 43 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(token.Nickname, "Detail", "Account", new { name = token.Username }, null));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

WriteLiteral("                        <li");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

            
            #line 44 "..\..\Views\Shared\_Layout.cshtml"
                                Write(Html.ActionLink(SR.Shared_Logout, "Logout", "Account", new { ReturnUrl = ViewContext.HttpContext.Request.Url.PathAndQuery }, null));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 45 "..\..\Views\Shared\_Layout.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </ul>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div" +
"");

WriteLiteral(" class=\"container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Views\Shared\_Layout.cshtml"
   Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"container footer\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"page-divider\"");

WriteLiteral("></div>\r\n        <div");

WriteLiteral(" class=\"clearfix\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"pull-right btn-group dropup\"");

WriteLiteral(">\r\n                <a");

WriteLiteral(" class=\"dropdown-toggle\"");

WriteLiteral(" data-toggle=\"dropdown\"");

WriteLiteral(" href=\"#\"");

WriteLiteral(">");

            
            #line 59 "..\..\Views\Shared\_Layout.cshtml"
                                                                      Write(ViewBag.Language);

            
            #line default
            #line hidden
WriteLiteral(" <span");

WriteLiteral(" class=\"caret\"");

WriteLiteral("></span></a>\r\n                <ul");

WriteLiteral(" class=\"dropdown-menu\"");

WriteLiteral(">\r\n                    <li>");

            
            #line 61 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.CultureActionLink("zh-cn"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n                    <li>");

            
            #line 62 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.CultureActionLink("en-us"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n                    <li>");

            
            #line 63 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.CultureActionLink("fr-fr"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n                </ul>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"pull-left\"");

WriteLiteral("><p");

WriteLiteral(" class=\"muted\"");

WriteAttribute("title", Tuple.Create(" title=\"", 3218), Tuple.Create("\"", 3251)
            
            #line 66 "..\..\Views\Shared\_Layout.cshtml"
, Tuple.Create(Tuple.Create("", 3226), Tuple.Create<System.Object, System.Int32>(Profiler.Current.Elapsed
            
            #line default
            #line hidden
, 3226), false)
);

WriteLiteral(">&copy; 2002-");

            
            #line 66 "..\..\Views\Shared\_Layout.cshtml"
                                                                                             Write(DateTime.Now.Year);

            
            #line default
            #line hidden
WriteLiteral(" 新生命开发团队</p></div>\r\n        </div>\r\n    </div>\r\n\r\n");

WriteLiteral("    ");

            
            #line 70 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 71 "..\..\Views\Shared\_Layout.cshtml"
Write(RenderSection("scripts", required: false));

            
            #line default
            #line hidden
WriteLiteral("\r\n</body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
