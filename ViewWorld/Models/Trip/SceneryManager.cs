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

namespace ViewWorld.Models.Trip
{
    public class SceneryManager : Controller
    {
        public delegate dynamic CustomQueryDelegate();
        public Scenery GetScenery(string sceneId,ApplicationIdentityContext db)
        {
            if (!string.IsNullOrWhiteSpace(sceneId))
            {
                return db.DB.Table("Sceneries").Get(sceneId).RunAtom<Scenery>(db.Connection);
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
                return await db.DB.Table("Sceneries").Get(SceneId).RunAtomAsync<Scenery>(db.Connection);
            }
            return null;
        }
        public Scenery AddScenery(Scenery scene, ApplicationIdentityContext db)
        {
            if (ModelState.IsValid)
            {
                return db.DB.Table("Sceneries").Insert(scene).RunAtom<Scenery>(db.Connection);
            }
            return scene;
        }

        public Result AddScenery(List<Scenery> sceneList, ApplicationIdentityContext db)
        {
            if (sceneList.Count() > 0)
            {
                return db.DB.Table("Sceneries").Insert(sceneList.ToArray()).RunResult<Result>(db.Connection);
            }
            return null;
        }
    }
}