using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using RethinkDb.Driver.Linq;

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
            var scene = db.DB.Table<Scenery>("Sceneries", db.Connection).Where(s => s.Popularity == 0).ToList();
            return Json(scene);
        }
    }
}