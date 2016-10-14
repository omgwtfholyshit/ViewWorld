using System;
using System.Threading.Tasks;
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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