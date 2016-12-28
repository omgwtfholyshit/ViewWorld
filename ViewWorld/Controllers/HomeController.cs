using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Utils;
using System.Threading.Tasks;
using ViewWorld.Core.Models.TripModels;
using CacheManager.Core;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;
using ViewWorld.Services.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ViewWorld.Controllers
{
    public class HomeController : BaseController
    {
        ICacheManager<object> cache;
        private readonly IMongoDbRepository Repo;
        readonly IAuthService AuthService;
        public HomeController(ICacheManager<object> _cache,IMongoDbRepository _repo, IAuthService _auth)
        {
            this.cache = _cache;
            this.Repo = _repo;
            this.AuthService = _auth;
        }
        public ActionResult Index()
        {
            return View();
        }
        [RequirePermission(Permission ="Region,Trip")]
        public ActionResult AddToCacheTest()
        {
            cache.Add("test", "testValue");
            return Content(cache.Get("test").ToString());
        }
        [RequirePermission(Permission = "Trip")]
        public ActionResult About()
        {
            HttpHelper.RequestUserLocation("118.19.3.42");
            return View();
        }
        public async Task<ActionResult> AddPermission(string name)
        {
            FilterDefinition<PermissionStore> fil = Builders<PermissionStore>.Filter.Eq("Permission.Name", name);
            var permission = await Repo.GetOneAsync<PermissionStore>(fil);
            var result = await AuthService.AddOnePermissionToUserAsync(this.UserId, permission.Entity.Permission);
            return Json(result);
        }
        public async Task<ActionResult> DeletePermission(string name)
        {
            return Json(await AuthService.DeleteOnePermissionFromUserAsync(this.UserId, name));
        }
        public ActionResult GetUserPermission()
        {
            var user = Repo.GetOne<ApplicationUser>(UserId);
            return Json(user.Entity);
        }
        public ActionResult Contact()
        {
            
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
            var userIdentity = User as ViewWorldPrincipal;
            return Json("hello");
        }

        public async Task<ActionResult> InsertPermissions()
        {
            var r = AuthService.AddOnePermissionAsync(new PermissionStore { Id = ObjectId.GenerateNewId().ToString(), Permission = new Permission { Name = "Region", ChineseName = "区域管理", Description = "管理区域分类的权限" } });
            var latest = await Repo.GetAllAsync<PermissionStore>();
            return Json(latest.Entities);
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
            await Repo.AddOneAsync(scene);
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
        public ActionResult PermissionRequired()
        {
            return Content("您没有权限执行该操作");
        }
    }
}