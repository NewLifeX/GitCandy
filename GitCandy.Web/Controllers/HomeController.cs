using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;

namespace GitCandy.Web.Controllers;

public class HomeController : CandyControllerBase
{
    public ActionResult Index() => RedirectToStartPage();

    [AllowAnonymous]
    public ActionResult About() => View();

    [AllowAnonymous]
    public ActionResult Language(String lang)
    {
        Response.Cookies.Append("Lang", lang);

        //Session["Culture"] = null;

        var url = Request.GetReferer();
        return url.IsNullOrEmpty() ? RedirectToStartPage() : Redirect(url);
    }
}