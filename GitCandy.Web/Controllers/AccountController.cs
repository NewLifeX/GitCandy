using GitCandy.Base;
using GitCandy.Configuration;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Data;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

public class AccountController : CandyControllerBase
{
    public ActionResult Index(String query, Int32? page)
    {
        if (!Token.IsAdmin()) return Forbid();

        var model = MembershipService.GetUserList(query, page ?? 1, UserConfiguration.Current.PageSize);

        ViewBag.Pager = Pager.Items(model.ItemCount)
            .PerPage(UserConfiguration.Current.PageSize)
            .Move(model.CurrentPage)
            .Segment(5)
            .Center();

        return View(model);
    }

    public ActionResult Detail(String name)
    {
        if (name.IsNullOrEmpty()) name = Token?.Name;

        var model = MembershipService.GetUserModel(name, true, Token?.Name);
        if (model == null) return NotFound(name);

        return View(model);
    }

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