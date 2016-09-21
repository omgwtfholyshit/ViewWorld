using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViewWorld.Controllers.Backend.Pages
{
    public class PageController : BaseController
    {
        // GET: Page
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserProfile()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
    }
}