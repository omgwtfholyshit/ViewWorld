using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Utils;
using MongoDB.Driver;
using MongoDB.Bson;
using ViewWorld.Models;
using System.Threading.Tasks;
using ViewWorld.Models.Managers;

namespace ViewWorld.Controllers
{
    public class HomeController : BaseController
    {
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
            return Json("hello");
        }

        public ActionResult TotalCollections()
        {
            //var collection = db.DB.ListCollections();
            //return Json(collection.ToList().Count());
            return null;
        }

        public async Task InsertOne(string name)
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
                Publisher = name,
                PublishedAt = DateTime.Now,
                Modificator = name,
            };
            scene.Id = Tools.GenerateId_M2();
            await Repo.AddOne(scene);
        }

        public void InsertMany()
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
            //db.DB.GetCollection<Scenery>("Scenerys").InsertMany(scenelist);
        }        



       

    }
}