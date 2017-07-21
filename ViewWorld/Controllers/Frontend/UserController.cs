using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Enum;
using ViewWorld.Services.Cities;
using ViewWorld.Services.Trips;
using ViewWorld.Services.Users;

namespace ViewWorld.Controllers.Frontend
{
    public class UserController : BaseController
    {
        #region constructor
        ICacheManager<object> cacheManager;
        ICityService cityService;
        ITripService tripService;
        IUserService userService;
        public UserController(ICacheManager<object> _cache, ICityService _city, ITripService _trip, IUserService _user)
        {
            cacheManager = _cache;
            cityService = _city;
            tripService = _trip;
            userService = _user;
        }
        #endregion
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddToCollection(string itemId, string itemName, string memo, ProductType type)
        {
            var result = await userService.AddToCollection(UserId, itemId, itemName, memo, type);
            return OriginJson(result);
        }
        [HttpGet]
        public async Task<JsonResult> CheckIfItemCollected(string itemId)
        {
            return Json(await userService.CheckIfItemCollected(UserId, itemId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveFromCollection(string itemId)
        {
            return Json(await userService.RemoveFromCollection(UserId, itemId));
        }
    }
}