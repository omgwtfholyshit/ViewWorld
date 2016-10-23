using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewWorld.App_Start;
using ViewWorld.Core.Models;

namespace ViewWorld.Models.Managers
{
    public class SceneryManager : Controller
    {
        public delegate dynamic CustomQueryDelegate();
        public async Task<Scenery> GetSceneryAsync(string SceneId, ApplicationIdentityContext db)
        {
            if (!string.IsNullOrWhiteSpace(SceneId))
            {
                return await db.DB.GetCollection<Scenery>("Sceneries").AsQueryable().Where(s => s.Id == SceneId).FirstAsync();
            }
            return null;
        }
        public Scenery AddScenery(Scenery scene, ApplicationIdentityContext db)
        {
            if (ModelState.IsValid)
            {
                throw new NotImplementedException();
            }
            return scene;
        }

    }
}