using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.App_Start;
using ViewWorld.Core.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ViewWorld.Models.Trip
{
    public class SceneryManager : Controller
    {
        public delegate dynamic CustomQueryDelegate();
        public Scenery GetScenery(string sceneId,ApplicationIdentityContext db)
        {
            if (!string.IsNullOrWhiteSpace(sceneId))
            {
                return db.DB.GetCollection<Scenery>("Sceneries").Find(document => document.Id == sceneId).FirstOrDefault();
            }
            return null;
        }
        public dynamic GetScenery(CustomQueryDelegate queryDelegate)
        {
            return queryDelegate.Invoke();
        }
        public async Task<Scenery> GetSceneryAsync(string SceneId, ApplicationIdentityContext db)
        {
            if (!string.IsNullOrWhiteSpace(SceneId))
            {
                return await db.DB.GetCollection<Scenery>("Sceneries").Find(document => document.Id == SceneId).SingleAsync();
            }
            return null;
        }
        public Scenery AddScenery(Scenery scene, ApplicationIdentityContext db)
        {
            if (ModelState.IsValid)
            {
                db.DB.GetCollection<Scenery>("Sceneries").InsertOne(scene);
            }
            return scene;
        }

        public Result AddScenery(List<Scenery> sceneList, ApplicationIdentityContext db)
        {
            if (sceneList.Count() > 0)
            {
                db.DB.GetCollection<Scenery>("Sceneries").InsertMany(sceneList.ToArray());
            }
            return null;
        }
    }
}