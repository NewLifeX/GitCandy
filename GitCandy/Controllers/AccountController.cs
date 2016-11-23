using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GitCandy.App_GlobalResources;
using GitCandy.Base;
using GitCandy.Configuration;
using GitCandy.Filters;
using GitCandy.Models;
using GitCandy.Security;
using NewLife.Log;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Controllers
{
    public class AccountController : CandyControllerBase
    {
        [Administrator]
        public ActionResult Index(String query, int? page)
        {
            var model = MembershipService.GetUserList(query, page ?? 1, UserConfiguration.Current.PageSize);

            ViewBag.Pager = Pager.Items(model.ItemCount)
                .PerPage(UserConfiguration.Current.PageSize)
                .Move(model.CurrentPage)
                .Segment(5)
                .Center();

            return View(model);
        }

        [AllowAnonymous]
        [AllowRegisterUser]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowRegisterUser]
        public ActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserX.Create(model.Name, model.Nickname, model.Password, model.Email, model.Description);
                    if (user != null)
                    {
                        if (Token != null) return RedirectToAction("Detail", "Account", new { name = user.Name });

                        var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
                        Token = new Token(auth.AuthCode, user.ID, user.Name, user.Nickname, user.IsAdmin);
                        return RedirectToStartPage();
                    }
                }
                catch (ArgumentException aex)
                {
                    ModelState.AddModelError(aex.ParamName, aex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult Detail(String name)
        {
            if (String.IsNullOrEmpty(name) && Token != null)
                name = Token.Username;

            var model = MembershipService.GetUserModel(name, true, Token?.Username);
            if (model == null)
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout(String returnUrl)
        {
            Token = null;
            return RedirectToStartPage(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Login(String returnUrl)
        {
            if (Token != null)
                return RedirectToStartPage(returnUrl);

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, String returnUrl)
        {
            var user = MembershipService.Login(model.ID, model.Password);
            if (user != null)
            {
                var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
                Token = new Token(auth.AuthCode, user.ID, user.Name, user.Nickname, user.IsAdmin);

                return RedirectToStartPage(returnUrl);
            }

            ModelState.AddModelError("", SR.Account_LoginFailed);
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [CurrentUserOrAdministrator]
        public ActionResult Change()
        {
            return View();
        }

        [HttpPost]
        [CurrentUserOrAdministrator]
        public ActionResult Change(ChangePasswordModel model, String name)
        {
            if (String.IsNullOrEmpty(name)) name = Token.Username;

            var isAdmin = Token.IsSystemAdministrator && !String.Equals(name, Token.Username, StringComparison.OrdinalIgnoreCase);
            if (ModelState.IsValid)
            {
                var user = UserX.Check(isAdmin ? Token.Username : name, model.OldPassword);
                if (user != null)
                {
                    MembershipService.SetPassword(name, model.NewPassword);
                    if (!isAdmin)
                    {
                        var auth = MembershipService.CreateAuthorization(user.ID, Token.AuthorizationExpires, Request.UserHostAddress);
                        Token = new Token(auth.AuthCode, user.ID, user.Name, user.Nickname, user.IsAdmin);
                    }

                    return RedirectToAction("Detail", "Account", new { name });
                }
                ModelState.AddModelError("OldPassword", SR.Account_OldPasswordError);
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Forgot()
        {
            return View();
        }

        [CurrentUserOrAdministrator]
        public ActionResult Edit(String name)
        {
            if (String.IsNullOrEmpty(name))
                name = Token.Username;

            var model = MembershipService.GetUserModel(name);
            if (model == null)
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
            ModelState.Clear();

            return View(model);
        }

        [HttpPost]
        [CurrentUserOrAdministrator]
        public ActionResult Edit(String name, UserModel model)
        {
            if (String.IsNullOrEmpty(name)) name = Token.Username;

            var isAdmin = Token.IsSystemAdministrator && !String.Equals(name, Token.Username, StringComparison.OrdinalIgnoreCase);

            ModelState.Remove("ConformPassword");
            if (ModelState.IsValid)
            {
                var user = UserX.Check(isAdmin ? Token.Username : name, model.Password);
                if (user != null)
                {
                    model.IsSystemAdministrator = Token.IsSystemAdministrator && model.IsSystemAdministrator;
                    if (!Token.IsSystemAdministrator || isAdmin || model.IsSystemAdministrator)
                    {
                        if (!MembershipService.UpdateUser(model))
                            throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
                        if (!isAdmin)
                        {
                            Token = MembershipService.GetToken(Token.AuthCode);
                        }

                        return RedirectToAction("Detail", "Account", new { name });
                    }
                    ModelState.AddModelError("IsSystemAdministrator", SR.Account_CantRemoveSelf);
                }
                else
                    ModelState.AddModelError("Password", SR.Account_PasswordError);
            }
            return View(model);
        }

        [HttpPost]
        [CurrentUserOrAdministrator]
        public JsonResult ChooseSsh(String user, String sshkey, String act)
        {
            String message = null;
            if (act == "add")
            {
                var fingerprint = MembershipService.AddSshKey(user, sshkey);
                if (fingerprint != null)
                    return Json(fingerprint);
            }
            else if (act == "del")
            {
                MembershipService.DeleteSshKey(user, sshkey);
                return Json("success");
            }

            Response.StatusCode = 400;
            return Json(message ?? SR.Shared_SomethingWrong);
        }

        [CurrentUserOrAdministrator]
        public ActionResult Ssh(String name)
        {
            if (String.IsNullOrEmpty(name) && Token != null)
                name = Token.Username;

            var model = MembershipService.GetSshList(name);
            if (model == null)
                throw new HttpException((int)HttpStatusCode.NotFound, String.Empty);
            return View(model);
        }

        [Administrator]
        public ActionResult Delete(String name, String conform)
        {
            if (String.Equals(Token.Username, name, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", SR.Account_CantRemoveSelf);
            }
            else if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
            {
                MembershipService.DeleteUser(name);
                XTrace.WriteLine("User {0} deleted by {1}#{2}", name, Token.Username, Token.UserID);
                return RedirectToAction("Index");
            }
            return View((object)name);
        }

        [HttpPost]
        public JsonResult Search(String query)
        {
            var result = MembershipService.SearchUsers(query);
            return Json(result);
        }
    }
}
