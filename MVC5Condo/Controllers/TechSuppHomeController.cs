using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5Condo.Controllers
{
    public class TechSuppHomeController : Controller
    {
        //
        // GET: /TechSuppHome/
        public ActionResult Index()
        {
            ViewBag.Message = "Tech Support Home Page.";

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
    }
}