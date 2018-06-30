﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GitCandy.Base;
using GitCandy.Configuration;
using GitCandy.Filters;
using NewLife.Data;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Controllers
{
    public class AccountController : CandyControllerBase
    {
        [Administrator]
        public ActionResult Index(String query, Int32? page)
        {
            var model = MembershipService.GetUserList(query, page ?? 1, UserConfiguration.Current.PageSize);

            ViewBag.Pager = Pager.Items(model.ItemCount)
                .PerPage(UserConfiguration.Current.PageSize)
                .Move(model.CurrentPage)
                .Segment(5)
                .Center();

            return View(model);
        }

        //[AllowAnonymous]
        //[AllowRegisterUser]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[AllowRegisterUser]
        //public ActionResult Create(UserModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var user = UserX.Create(model.Name, model.Nickname, model.Password, model.Email, model.Description);
        //            if (user != null)
        //            {
        //                if (Token != null) return RedirectToAction("Detail", "Account", new { name = user.Name });

        //                var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
        //                Token = new Token(auth.AuthCode, user.ID, user.Name, user.NickName, user.IsAdmin);
        //                return RedirectToStartPage();
        //            }
        //        }
        //        catch (ArgumentException aex)
        //        {
        //            ModelState.AddModelError(aex.ParamName, aex.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", ex.Message);
        //        }
        //    }

        //    return View(model);
        //}

        public ActionResult Detail(String name)
        {
            if (name.IsNullOrEmpty()) name = Token?.Name;

            var model = MembershipService.GetUserModel(name, true, Token?.Name);
            if (model == null)
                throw new HttpException((Int32)HttpStatusCode.NotFound, String.Empty);
            return View(model);
        }

        //[AllowAnonymous]
        //public ActionResult Logout(String returnUrl)
        //{
        //    Token = null;
        //    return RedirectToStartPage(returnUrl);
        //}

        //[AllowAnonymous]
        //public ActionResult Login(String returnUrl)
        //{
        //    if (Token != null) return RedirectToStartPage(returnUrl);

        //    ViewBag.ReturnUrl = returnUrl;

        //    // 只有一个管理员时，显示默认用户名密码
        //    if (UserX.Meta.Count == 1)
        //    {
        //        // 如果没有修改默认密码，则就用它显示
        //        var user = UserX.Meta.Cache.Entities.ToList().FirstOrDefault();
        //        if (user != null && user.Password == user.Name.MD5())
        //        {
        //            ViewBag.ShowAdmin = user.Name;
        //        }
        //    }

        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Login(LoginModel model, String returnUrl)
        //{
        //    var user = MembershipService.Login(model.ID, model.Password);
        //    if (user != null)
        //    {
        //        var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
        //        Token = new Token(auth.AuthCode, user.ID, user.Name, user.NickName, user.IsAdmin);

        //        // 设置上下文


        //        return RedirectToStartPage(returnUrl);
        //    }

        //    ModelState.AddModelError("", SR.Account_LoginFailed);
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //[CurrentUserOrAdministrator]
        //public ActionResult Change()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[CurrentUserOrAdministrator]
        //public ActionResult Change(ChangePasswordModel model, String name)
        //{
        //    if (String.IsNullOrEmpty(name)) name = Token?.Name;

        //    var isAdmin = Token.IsAdmin && !String.Equals(name, Token?.Name, StringComparison.OrdinalIgnoreCase);
        //    if (ModelState.IsValid)
        //    {
        //        var user = UserX.Check(isAdmin ? Token?.Name : name, model.OldPassword);
        //        if (user != null)
        //        {
        //            MembershipService.SetPassword(name, model.NewPassword);
        //            if (!isAdmin)
        //            {
        //                var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
        //                Token = new Token(auth.AuthCode, user.ID, user.Name, user.NickName, user.IsAdmin);
        //            }

        //            return RedirectToAction("Detail", "Account", new { name });
        //        }
        //        ModelState.AddModelError("OldPassword", SR.Account_OldPasswordError);
        //    }
        //    return View(model);
        //}

        //[AllowAnonymous]
        //public ActionResult Forgot()
        //{
        //    return View();
        //}

        //[CurrentUserOrAdministrator]
        //public ActionResult Edit(String name)
        //{
        //    if (name.IsNullOrEmpty()) name = Token?.Name;

        //    var model = MembershipService.GetUserModel(name);
        //    if (model == null)
        //        throw new HttpException((Int32)HttpStatusCode.NotFound, String.Empty);
        //    ModelState.Clear();

        //    return View(model);
        //}

        //[HttpPost]
        //[CurrentUserOrAdministrator]
        //public ActionResult Edit(String name, UserModel model)
        //{
        //    ModelState.Remove("ConformPassword");
        //    if (!ModelState.IsValid) return View(model);

        //    if (String.IsNullOrEmpty(name)) name = Token?.Name;

        //    UserX user = null;
        //    // 管理员直接修改任何人，否则只能改自己
        //    if (Token.IsAdmin())
        //    {
        //        user = UserX.Check(Token?.Name, model.Password);
        //        if (user != null)
        //        {
        //            // 要修改的目标用户
        //            user = UserX.FindByName(name);

        //            user.IsAdmin = model.IsAdmin;
        //        }
        //    }
        //    else
        //    {
        //        user = UserX.Check(name, model.Password);
        //    }

        //    // 验证成功
        //    if (user != null)
        //    {
        //        user.NickName = model.Nickname;
        //        user.Email = model.Email;
        //        user.Description = model.Description;

        //        user.Save();

        //        //// 如果修改的是自己，则需要重新加载，而不论是否管理员
        //        //if (Token?.Name == name) Token = MembershipService.GetToken(Token.AuthCode);

        //        return RedirectToAction("Detail", "Account", new { name });
        //    }


        //    ModelState.AddModelError("Password", SR.Account_PasswordError);

        //    return View(model);
        //}

        //[HttpPost]
        //[CurrentUserOrAdministrator]
        //public JsonResult ChooseSsh(String user, String sshkey, String act)
        //{
        //    String message = null;
        //    if (act == "add")
        //    {
        //        var fingerprint = MembershipService.AddSshKey(user, sshkey);
        //        if (fingerprint != null)
        //            return Json(fingerprint);
        //    }
        //    else if (act == "del")
        //    {
        //        MembershipService.DeleteSshKey(user, sshkey);
        //        return Json("success");
        //    }

        //    Response.StatusCode = 400;
        //    return Json(message ?? SR.Shared_SomethingWrong);
        //}

        //[CurrentUserOrAdministrator]
        //public ActionResult Ssh(String name)
        //{
        //    if (String.IsNullOrEmpty(name) && Token != null)
        //        name = Token?.Name;

        //    var model = MembershipService.GetSshList(name);
        //    if (model == null)
        //        throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
        //    return View(model);
        //}

        //[Administrator]
        //public ActionResult Delete(String name, String conform)
        //{
        //    if (String.Equals(Token?.Name, name, StringComparison.OrdinalIgnoreCase))
        //    {
        //        ModelState.AddModelError("", SR.Account_CantRemoveSelf);
        //    }
        //    else if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
        //    {
        //        MembershipService.DeleteUser(name);
        //        XTrace.WriteLine("User {0} deleted by {1}#{2}", name, Token?.Name, Token.UserID);
        //        return RedirectToAction("Index");
        //    }
        //    return View((Object)name);
        //}

        [HttpPost]
        public JsonResult Search(String query)
        {
            var p = new PageParameter
            {
                PageSize = 20
            };

            var list = UserX.SearchUser(query, p);
            var ns = list.ToList().Select(e => e.Name).ToArray();

            return Json(ns);
        }
    }
}