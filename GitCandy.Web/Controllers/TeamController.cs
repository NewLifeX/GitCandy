using GitCandy.Base;
using GitCandy.Models;
using GitCandy.Web.App_GlobalResources;
using GitCandy.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Log;
using UserX = NewLife.GitCandy.Entity.User;

namespace GitCandy.Web.Controllers;

public class TeamController : CandyControllerBase
{
    public ActionResult Index(String query, Int32? page)
    {
        if (!Token.IsAdmin()) return Forbid();

        var model = MembershipService.GetTeamList(query, page ?? 1, GitSetting.Current.PageSize);

        ViewBag.Pager = Pager.Items(model.ItemCount)
            .PerPage(GitSetting.Current.PageSize)
            .Move(model.CurrentPage)
            .Segment(5)
            .Center();

        return View(model);
    }

    public ActionResult Create()
    {
        if (!Token.IsAdmin()) return Forbid();

        return View();
    }

    [HttpPost]
    public ActionResult Create(TeamModel model)
    {
        if (!Token.IsAdmin()) return Forbid();

        if (ModelState.IsValid)
        {
            try
            {
                var team = UserX.CreateTeam(model.Name, model.Nickname, model.Description);
                if (team != null)
                    return RedirectToAction("Detail", "Team", new { team.Name });
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
        if (!Token.IsAdmin())
        {
            var user = Token as UserX;
            var ut = user?.Teams.FirstOrDefault(e => e.TeamName.EqualIgnoreCase(name));
            if (ut == null) return Forbid();
        }

        var model = MembershipService.GetTeamModel(name, true, Token?.Name);
        if (model == null) return NotFound();

        return View(model);
    }

    public ActionResult Edit(String name)
    {
        if (!Token.IsAdmin())
        {
            var user = Token as UserX;
            var ut = user?.Teams.FirstOrDefault(e => e.TeamName.EqualIgnoreCase(name));

            if (ut == null || !ut.IsAdmin) return Forbid();
        }

        var model = MembershipService.GetTeamModel(name);
        if (model == null) return NotFound();

        ModelState.Clear();

        return View(model);
    }

    [HttpPost]
    public ActionResult Edit(String name, TeamModel model)
    {
        if (!Token.IsAdmin())
        {
            var user = Token as UserX;
            var ut = user?.Teams.FirstOrDefault(e => e.TeamName.EqualIgnoreCase(name));

            if (ut == null || !ut.IsAdmin) return Forbid();
        }

        if (ModelState.IsValid && !MembershipService.UpdateTeam(model)) return NotFound();

        return View(model);
    }

    public ActionResult Users(String name)
    {
        if (!Token.IsAdmin())
        {
            var user = Token as UserX;
            var ut = user?.Teams.FirstOrDefault(e => e.TeamName.EqualIgnoreCase(name));

            if (ut == null) return Forbid();
        }

        var model = MembershipService.GetTeamModel(name, true);
        return View(model);
    }

    [HttpPost]
    public ActionResult ChooseUser(String name, String user, String act)
    {
        if (!Token.IsAdmin())
        {
            var ut = (Token as UserX)?.Teams.FirstOrDefault(e => e.TeamName.EqualIgnoreCase(name));

            if (ut == null) return Forbid();
        }

        String message = null;
        if (act == "add")
        {
            if (MembershipService.TeamAddUser(name, user)) return Json("success");

            if (act == "del")
            {
                if (String.Equals(user, Token?.Name, StringComparison.OrdinalIgnoreCase))
                    message = SR.Account_CantRemoveSelf;
                else if (MembershipService.TeamRemoveUser(name, user))
                    return Json("success");
                else if (act is "admin" or "member")
                {
                    var isAdmin = act == "admin";
                    if (!isAdmin && String.Equals(user, Token?.Name, StringComparison.OrdinalIgnoreCase))
                        message = SR.Account_CantRemoveSelf;
                    else if (MembershipService.TeamUserSetAdministrator(name, user, isAdmin))
                        return Json("success");
                }
            }
        }

        Response.StatusCode = 400;
        return Json(message ?? SR.Shared_SomethingWrong);
    }

    public ActionResult Delete(String name, String conform)
    {
        if (!Token.IsAdmin()) return Forbid();

        if (String.Equals(conform, "yes", StringComparison.OrdinalIgnoreCase))
        {
            MembershipService.DeleteTeam(name);
            XTrace.WriteLine("Team {0} deleted by {1}#{2}", name, Token?.Name, Token.ID);
            return RedirectToAction("Index");
        }
        return View((Object)name);
    }

    [HttpPost]
    public JsonResult Search(String query)
    {
        var result = MembershipService.SearchTeam(query);
        return Json(result);
    }
}