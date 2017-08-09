using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.BusinessModels;
using ViewWorld.Services.Cities;
using ViewWorld.Services.Order;
using ViewWorld.Services.Trips;
using ViewWorld.Services.Users;

namespace ViewWorld.Controllers.Frontend
{
    [Authorize]
    public class UserController : BaseController
    {
        #region constructor
        ICacheManager<object> cacheManager;
        ICityService cityService;
        ITripService tripService;
        IUserService userService;
        IOrderService orderService;
        public UserController(ICacheManager<object> _cache, ICityService _city, ITripService _trip, IUserService _user, IOrderService _order)
        {
            cacheManager = _cache;
            cityService = _city;
            tripService = _trip;
            userService = _user;
            orderService = _order;
        }
        #endregion
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        #region Collection
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
        #endregion
        #region Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddToOrder(BusinessOrder order)
        {
            var result = new Result() { ErrorCode = 300, Message = "数据模型错误", Success = false };
            if (ModelState.IsValid)
            {
                order.UserId = UserId;
                order.Type = ProductType.旅行团订单;
                result = await orderService.AddEntity(order);
            }
            return OriginJson(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteOrder(string orderId)
        {
            return OriginJson(await orderService.DeleteEntityById(orderId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateOrder(BusinessOrder order)
        {
            var result = new Result() { ErrorCode = 300, Message = "数据模型错误", Success = false };
            if (ModelState.IsValid)
            {
                order.LastModifiedAt = DateTime.Now;
                order.ModificatorId = UserId;
                order.ModificatorName = User.Identity.Name;
                result = await orderService.UpdateEntity(order);
            }
            return OriginJson(result);
        }
        [HttpGet]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Server,  Duration = 20)]
        public async Task<JsonResult> GetOrderCount()
        {
            return Json(await orderService.CountOrders(UserId));
        }
        #endregion
    }
}