using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5Condo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Login to see how your condo miners are doing.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is the Centerus Condo Management System.";

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Centerus Inc Contact";

            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Message = "xx"; // "Owner";
        
            return View();
        }
    }
}