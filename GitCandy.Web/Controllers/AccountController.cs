using GitCandy.Base;
using GitCandy.Models;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Data;
using XCode.Membership;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

/// <summary>账号控制器。提供用户列表查看、用户详情和用户名搜索</summary>
public class AccountController : CandyControllerBase
{
    /// <summary>用户列表。仅管理员可查看</summary>
    /// <param name="query">搜索关键词</param>
    /// <param name="page">页码</param>
    /// <returns></returns>
    public ActionResult Index(String query, Int32? page)
    {
        if (!Token.IsAdmin()) return Forbid();

        var model = MembershipService.GetUserList(query, page ?? 1, GitSetting.Current.PageSize);

        ViewBag.Pager = Pager.Items(model.ItemCount)
            .PerPage(GitSetting.Current.PageSize)
            .Move(model.CurrentPage)
            .Segment(5)
            .Center();

        return View(model);
    }

    /// <summary>用户详情页。查看用户个人信息、所属团队和仓库列表</summary>
    /// <param name="name">用户名</param>
    /// <returns></returns>
    public ActionResult Detail(String name)
    {
        if (name.IsNullOrEmpty()) name = Token?.Name;

        var model = MembershipService.GetUserModel(name, true, Token?.Name);
        if (model == null) return NotFound(name);

        return View(model);
    }

    /// <summary>修改密码页面。仅本人或管理员可访问</summary>
    /// <param name="name">用户名</param>
    /// <returns></returns>
    public ActionResult Change(String name)
    {
        if (name.IsNullOrEmpty()) name = Token?.Name;
        if (!CanManageUser(name)) return Forbid();

        ViewBag.Name = name;
        return View(new ChangePasswordModel());
    }

    /// <summary>修改密码提交</summary>
    /// <param name="name">用户名</param>
    /// <param name="model">密码模型</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Change(String name, ChangePasswordModel model)
    {
        if (name.IsNullOrEmpty()) name = Token?.Name;
        if (!CanManageUser(name)) return Forbid();

        ViewBag.Name = name;
        if (!ModelState.IsValid) return View(model);

        try
        {
            var provider = ManageProvider.Provider;
            if (provider == null) throw new InvalidOperationException("用户提供者未初始化");

            provider.ChangePassword(name, model.NewPassword, model.OldPassword);
            return RedirectToAction("Detail", new { name });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    private Boolean CanManageUser(String name) => Token != null && (Token.IsAdmin() || Token.Name.EqualIgnoreCase(name));

    /// <summary>用户名搜索（AJAX）</summary>
    /// <param name="query">搜索关键词</param>
    /// <returns></returns>
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