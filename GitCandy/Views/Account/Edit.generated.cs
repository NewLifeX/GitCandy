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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Account/Edit.cshtml")]
    public partial class _Views_Account_Edit_cshtml : System.Web.Mvc.WebViewPage<GitCandy.Models.UserModel>
    {
        public _Views_Account_Edit_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Account\Edit.cshtml"
  
    ViewBag.Title = string.Format(SR.Shared_TitleFormat, string.Format(SR.Account_EditTitle, Model.Name));
    var token = GitCandy.Security.Token.Current;

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<h3>");

            
            #line 8 "..\..\Views\Account\Edit.cshtml"
Write(string.Format(SR.Account_EditTitle, Model.Name));

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n\r\n\r\n");

            
            #line 11 "..\..\Views\Account\Edit.cshtml"
 using (Html.BeginForm("Edit", "Account", FormMethod.Post))
{

            
            #line default
            #line hidden
WriteLiteral("    <dl");

WriteLiteral(" class=\"dl-horizontal col-md-8\"");

WriteLiteral(">\r\n        <dt>");

            
            #line 14 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.Name));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n        <dd>\r\n");

WriteLiteral("            ");

            
            #line 16 "..\..\Views\Account\Edit.cshtml"
       Write(Html.HiddenFor(s => s.Name));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 17 "..\..\Views\Account\Edit.cshtml"
       Write(Model.Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>");

            
            #line 20 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.Password));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n        <dd>");

            
            #line 21 "..\..\Views\Account\Edit.cshtml"
       Write(Html.PasswordFor(s => s.Password, new { @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n        <dd>\r\n            <span");

WriteLiteral(" class=\"text-danger\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 24 "..\..\Views\Account\Edit.cshtml"
           Write(Html.ValidationMessageFor(s => s.Password));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n        </dd>\r\n\r\n        <dt>");

            
            #line 28 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.Nickname));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n        <dd>");

            
            #line 29 "..\..\Views\Account\Edit.cshtml"
       Write(Html.TextBoxFor(s => s.Nickname, new { @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n        <dd>\r\n            <span");

WriteLiteral(" class=\"text-danger\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 32 "..\..\Views\Account\Edit.cshtml"
           Write(Html.ValidationMessageFor(s => s.Nickname));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n        </dd>\r\n\r\n        <dt>");

            
            #line 36 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.Email));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n        <dd>");

            
            #line 37 "..\..\Views\Account\Edit.cshtml"
       Write(Html.TextBoxFor(s => s.Email, new { @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n        <dd>\r\n            <span");

WriteLiteral(" class=\"text-danger\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 40 "..\..\Views\Account\Edit.cshtml"
           Write(Html.ValidationMessageFor(s => s.Email));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n        </dd>\r\n\r\n        <dt>");

            
            #line 44 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.Description));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n        <dd>");

            
            #line 45 "..\..\Views\Account\Edit.cshtml"
       Write(Html.TextAreaFor(s => s.Description, 4, 0, new { @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n        <dd>\r\n            <span");

WriteLiteral(" class=\"text-danger\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 48 "..\..\Views\Account\Edit.cshtml"
           Write(Html.ValidationMessageFor(s => s.Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n        </dd>\r\n\r\n");

            
            #line 52 "..\..\Views\Account\Edit.cshtml"
        
            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\Account\Edit.cshtml"
         if (token.IsSystemAdministrator)
        {

            
            #line default
            #line hidden
WriteLiteral("        <dt>");

            
            #line 54 "..\..\Views\Account\Edit.cshtml"
       Write(Html.DisplayNameFor(s => s.IsSystemAdministrator));

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n");

WriteLiteral("        <dd>\r\n            <div");

WriteLiteral(" class=\"switch\"");

WriteLiteral(" data-on-label=\"");

            
            #line 56 "..\..\Views\Account\Edit.cshtml"
                                          Write(SR.Shared_Yes);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(" data-off-label=\"");

            
            #line 56 "..\..\Views\Account\Edit.cshtml"
                                                                          Write(SR.Shared_No);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 57 "..\..\Views\Account\Edit.cshtml"
           Write(Html.CheckBoxFor(s => s.IsSystemAdministrator, new { data_size = "small" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </dd>\r\n");

            
            #line 60 "..\..\Views\Account\Edit.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("\r\n        <dt></dt>\r\n        <dd>");

            
            #line 63 "..\..\Views\Account\Edit.cshtml"
       Write(Html.ValidationSummary(true, SR.Account_UpdateUnsuccessfull, new { @class = "alert alert-dismissable alert-danger" }));

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n\r\n        <dt>\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\Account\Edit.cshtml"
       Write(Html.ActionLink(SR.Shared_Back, "Detail", new { Model.Name }, new { @class = "btn btn-default pull-left" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn btn-primary\"");

WriteLiteral(">");

            
            #line 69 "..\..\Views\Account\Edit.cshtml"
                                                     Write(SR.Shared_Save);

            
            #line default
            #line hidden
WriteLiteral("</button> &nbsp;\r\n            <button");

WriteLiteral(" type=\"reset\"");

WriteLiteral(" class=\"btn btn-inverse\"");

WriteLiteral(">");

            
            #line 70 "..\..\Views\Account\Edit.cshtml"
                                                    Write(SR.Shared_Reset);

            
            #line default
            #line hidden
WriteLiteral("</button> &nbsp;\r\n");

            
            #line 71 "..\..\Views\Account\Edit.cshtml"
            
            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\Account\Edit.cshtml"
             if (token != null && token.IsSystemAdministrator)
            {
                
            
            #line default
            #line hidden
            
            #line 73 "..\..\Views\Account\Edit.cshtml"
           Write(Html.ActionLink(SR.Shared_Delete, "Delete", new { Model.Name }, new { @class = "btn btn-danger" }));

            
            #line default
            #line hidden
            
            #line 73 "..\..\Views\Account\Edit.cshtml"
                                                                                                                   
            }

            
            #line default
            #line hidden
WriteLiteral("        </dd>\r\n    </dl>\r\n");

            
            #line 77 "..\..\Views\Account\Edit.cshtml"
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
