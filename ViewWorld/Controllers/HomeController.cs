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
using RethinkDb.Driver.Model;
using static ViewWorld.Models.Trip.SceneryManager;

namespace ViewWorld.Controllers
{
    public class HomeController : BaseController
    {
        TripManager tripManager;
        SceneryManager sceneryManager;
        public HomeController()
            :this(new TripManager(),new SceneryManager())
        {

        }
        public HomeController(TripManager _tripManager,SceneryManager _sceneryManager)
        {
            tripManager = _tripManager;
            sceneryManager = _sceneryManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            HttpHelper.RequestUserLocation("118.19.3.42");
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public Scenery CustQuery()
        {
            return db.DB.Table("Sceneries").Get("4800564445081176863").RunAtom<Scenery>(db.Connection);
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
                //Location = loc,
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
               // Location = loc,
                Name = "天安门旅行",
                Popularity = 0,
                Publisher = "刘震",
                PublishedAt = DateTime.Now,
                Modificator = "S2",
            };
            scene2.Id = Tools.GenerateId_M2();
            List<Scenery> scenelist = new List<Scenery>();
            scenelist.Add(scene);
            scenelist.Add(scene2);
            //var ts = sceneryManager.AddScenery(scenelist, db);
            var tss = sceneryManager.GetScenery(delegate { return db.DB.Table("Sceneries").Get("4800564445081176863").RunAtom<Scenery>(db.Connection); });
            return Json("hello");
        }
    }
}