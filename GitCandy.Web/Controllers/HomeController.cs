using System;
using System.Web;
using System.Web.Mvc;

namespace GitCandy.Controllers
{
    public class HomeController : CandyControllerBase
    {
        public ActionResult Index()
        {
            return RedirectToStartPage();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Language(String lang)
        {
            var cookie = new HttpCookie("Lang", lang);
            Response.Cookies.Set(cookie);

            Session["Culture"] = null;

            if (Request.UrlReferrer == null) return RedirectToStartPage();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
    }
}