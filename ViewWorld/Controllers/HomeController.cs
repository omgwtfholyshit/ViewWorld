using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Models;
using ViewWorld.Utils;
using RethinkDb.Driver;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Linq;
using ViewWorld.Models.Trip;

namespace ViewWorld.Controllers
{
    public class HomeController : BaseController
    {
        TripManager tripManager;
        public HomeController()
            :this(new TripManager())
        {

        }
        public HomeController(TripManager _tripManager)
        {
            tripManager = _tripManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TestMethods()
        {
            GeoPoint geo = new GeoPoint()
            {
                Latitude = "39.908718",
                Longitude = "116.397495"

            };
            Location loc = new Location()
            {
                Country = "中国",
                Province = "北京",
                District = "朝阳区",
                City = "北京市",
                Address = "天安门",
                PostCode = "100000"
            };
            Scenery scene = new Scenery()
            {
                Coordinate = geo,
                LastUpdateAt = DateTime.Now,
                Location = loc,
                Name = "天安门旅行",
                Popularity = 0,
                Publisher = "刘震",
                PublishedAt = DateTime.Now,
                Modificator = "刘震",
            };
            scene.Id = Tools.GenerateId_M2();
            Scenery scene2 = new Scenery()
            {
                Coordinate = geo,
                LastUpdateAt = DateTime.Now,
                Location = loc,
                Name = "天安门旅行",
                Popularity = 0,
                Publisher = "刘震",
                PublishedAt = DateTime.Now,
                Modificator = "S2",
            };
            scene2.Id = Tools.GenerateId_M2();
            Scenery[] itemlist = new Scenery[2];
            itemlist[0] = scene;
            itemlist[1] = scene2;
            var items = db.DB.Table("Sceneries").Insert(itemlist).Run<Scenery>(db.Connection);
            return Json(items);
        }
    }
}