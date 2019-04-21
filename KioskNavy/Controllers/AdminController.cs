using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KioskNavy.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            return View();
        }
        // GET: Admin
        [Authorize]
        public ActionResult AdminOptions()
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            return View();
        }
    }
}