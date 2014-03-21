using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OAuthAspnetMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Autodesk OAuth ASP.NET MVC sample!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
