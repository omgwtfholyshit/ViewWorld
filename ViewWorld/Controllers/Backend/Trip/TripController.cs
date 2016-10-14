using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using MongoDB.Driver;

namespace ViewWorld.Controllers.Trip
{
    public class TripController : BaseController
    {
        // GET: Trip
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestMethod()
        {
           
            return Json("1");
        }
    }
}