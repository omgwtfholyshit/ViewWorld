using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Utils;
using ViewWorld.Models.Trip;
using MongoDB.Driver;
using MongoDB.Bson;
using ViewWorld.Models;
using System.Threading.Tasks;

namespace ViewWorld.Controllers
{
    public class HomeController : BaseController
    {
        TripManager tripManager;
        SceneryManager sceneryManager;
        MongoRepository repo;
        public HomeController()
            :this(new TripManager(),new SceneryManager(),new MongoRepository())
        {

        }
        public HomeController(TripManager _tripManager,SceneryManager _sceneryManager,MongoRepository _repo)
        {
            tripManager = _tripManager;
            sceneryManager = _sceneryManager;
            repo = _repo;
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
            var collection = db.DB.ListCollections();
            return Json(collection.ToList().Count());
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
            await repo.AddOne(scene);
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
            db.DB.GetCollection<Scenery>("Scenerys").InsertMany(scenelist);
        }        

        public List<Scenery> FindAll()
        {
            var list = db.DB.GetCollection<Scenery>("Scenerys").Find(new BsonDocument()).ToList();
            return list;
        }

        public List<Scenery> FindFilter(string name)
        {            
            var filter = Builders<Scenery>.Filter.Eq("Publisher", name);
            var sort = Builders<Scenery>.Sort.Descending("PublishedAt");
            var document = db.DB.GetCollection<Scenery>("Scenerys").Find(filter).Sort(sort).ToList();
            return document;
        }

        public long updateScenery(string publisher, string attraction, string EnglishAttraction)
        {
            var filter = Builders<Scenery>.Filter.Eq("Publisher", publisher);
            var update = Builders<Scenery>.Update.Set("Name", attraction).Set("EnglishName", EnglishAttraction).CurrentDate("LastUpdateAt");            
            long modifiedCount = db.DB.GetCollection<Scenery>("Scenerys").UpdateMany(filter, update).ModifiedCount;
            return modifiedCount;
        }

        public async Task<ActionResult> MongoTest()
        {
            var list = FindAll();
            ViewBag.FindAll = list.ToJson();
            ViewBag.Total = list.Count();

           // await InsertOne("");
            ViewBag.InsertOne = FindAll().ToJson();
            ViewBag.TotalInsertOne = db.DB.GetCollection<Scenery>("Scenerys").Count(new BsonDocument());
            var repoTotal =await repo.Count<Scenery>(new BsonDocument());

            //InsertMany();
            ViewBag.InsertMany = FindAll().ToJson();
            ViewBag.TotalInsertMany = db.DB.GetCollection<Scenery>("Scenerys").Count(new BsonDocument());
            var repoMany =await repo.Count<Scenery>(new BsonDocument());
            // uncomment the following code when you have documents in Scenery colleciton.

            //Find&Update
            FilterDefinition<Scenery> fil = Builders<Scenery>.Filter.Eq("_id", "5509789136186508709");
            UpdateDefinition<Scenery> upd = Builders<Scenery>.Update.Set("Name", "Whatever").CurrentDate("LastUpdateAt");
            var res = await repo.UpdateOne(fil, upd);

            var result = await repo.GetOne(fil);
            //ViewBag.Publisher = "Kevin";
            //ViewBag.FindFilter = FindFilter("Kevin").ToJson();

            //ViewBag.UpdateSceneryTotalModified = updateScenery("Kevin", "悉尼歌剧院", "Opera House");
            //ViewBag.FindFilterAfterUpdate = FindFilter("Kevin").ToJson();
            return View();
        }
    }
}