using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Equipo()
        {
            ViewBag.Message = "SC-601 Programación Avanzada";

            return View();
        }
    }
}

