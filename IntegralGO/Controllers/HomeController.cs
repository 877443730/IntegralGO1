using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegralGO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Myself()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Myself_Class(string aa)
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.Itemclass = aa;
            return View();
        }
    }
}