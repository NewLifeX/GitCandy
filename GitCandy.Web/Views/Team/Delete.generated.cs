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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Team/Delete.cshtml")]
    public partial class _Views_Team_Delete_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_Team_Delete_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 1 "..\..\Views\Team\Delete.cshtml"
  
    ViewBag.Title = String.Format(SR.Shared_TitleFormat, String.Format(SR.Team_DeleteTitle, Model));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"panel panel-danger\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"panel-heading\"");

WriteLiteral(">\r\n        <h4");

WriteLiteral(" class=\"panel-title\"");

WriteLiteral(">");

            
            #line 7 "..\..\Views\Team\Delete.cshtml"
                           Write(SR.Shared_Conform);

            
            #line default
            #line hidden
WriteLiteral("</h4>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"panel-body\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 10 "..\..\Views\Team\Delete.cshtml"
   Write(String.Format(SR.Team_DeletionWords, Model));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n");

            
            #line 14 "..\..\Views\Team\Delete.cshtml"
Write(Html.ValidationSummary(true, "", new { @class = "alert alert-error" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div>\r\n    <div");

WriteLiteral(" class=\"pull-left\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\Team\Delete.cshtml"
   Write(Html.ActionLink(SR.Shared_Back, "Detail", "Team", new { name = Model }, new { @class = "btn btn-default" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"pull-right\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 21 "..\..\Views\Team\Delete.cshtml"
   Write(Html.ActionLink(SR.Shared_Delete, "Delete", "Team", new { name = Model, Conform = "Yes" }, new { @class = "btn btn-danger" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
