using CacheManager.Core;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        #region pages
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("MyOrders");
        }
        public ActionResult MyOrders()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> RenderOrderManagementPartial(string orderId, OrderStatus status = OrderStatus.行程已确认, ProductType type = ProductType.不限, int pageNum = 1)
        {
            var result = await orderService.GetOrder(status, type, UserId, orderId);
            List<BusinessOrder> list = new List<BusinessOrder>();
            if (result.Success)
                list = result.Entities;

            return PartialView("~/Views/PartialViews/_PartialOrderList.cshtml",list.OrderByDescending(o=>o.LastModifiedAt).ToPagedList(pageNum,4));
        }
        public async Task<ActionResult> FillOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return Content("订单号不能为空");
            }
            var result = await orderService.RetrieveOrdersById(orderId);
            if (result.Success)
            {
                if (result.Entity.UserId != UserId || result.Entity.Status != OrderStatus.新创建订单)
                {
                    return Content("您没有权限查看该订单");
                }
                else
                {
                    return View(result.Entity);
                }
            }
            return Content("找不到该订单");
        }
        public async Task<ActionResult> OrderDetail(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return Content("订单号不能为空");
            }
            var result = await orderService.RetrieveOrdersById(orderId);
            if (result.Success)
            {
                if(result.Entity.UserId != UserId)
                {
                    return Content("您没有权限查看该订单");
                }
                else
                {
                    if(result.Entity.Status == OrderStatus.新创建订单)
                    {
                        return RedirectToAction("FillOrder", "User", new { orderId = orderId });
                    }
                    return View(result.Entity);
                }
            }
            return Content("找不到该订单");
        }
        public ActionResult MyCollections()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> RenderCollectionPartial(int pageNum = 1)
        {
            List<Collection> collectionList = new List<Collection>();
            var result = await userService.GetCollection(UserId);
            if (result.Success)
            {
                collectionList = result.Entities.ToList();
            }
            return PartialView("~/Views/PartialViews/_PartialCollection.cshtml", collectionList.OrderByDescending(c => c.CollectedAt).ToPagedList(pageNum, 4));

        }
        #endregion
        #region Collection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddToCollection(string itemId, string itemName, string memo,string image, ProductType type)
        {
            var result = await userService.AddToCollection(UserId, itemId, itemName, memo, image, type);
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
        public async Task<JsonResult> AddToOrder(BusinessOrder order, bool bookingRequired = false)
        {
            var result = new Result() { ErrorCode = 300, Message = "数据模型错误", Success = false };
            if (ModelState.IsValid)
            {
                order.UserId = UserId;
                order.Type = ProductType.旅行团订单;
                if (bookingRequired)
                {
                    order.Status = OrderStatus.新创建订单;
                }
                else
                {
                    order.Status = OrderStatus.行程已确认;
                }
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