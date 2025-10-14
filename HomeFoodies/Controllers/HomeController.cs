using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeFoodies.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View("~/Views/Admin/Home/Index.cshtml");
            else
                return View("~/Views/Client/Home/Index.cshtml");
        }
    }
}