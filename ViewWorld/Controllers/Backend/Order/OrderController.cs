using CacheManager.Core;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.BusinessModels;
using ViewWorld.Core.Models.Identity;
using ViewWorld.Services.Order;
using ViewWorld.ViewModels;

namespace ViewWorld.Controllers.Backend.Order
{
    [Authorize(Roles ="管理员,销售")]
    public class OrderController : BaseController
    {
        ICacheManager<object> cacheManager;
        IOrderService orderService;
        public OrderController(ICacheManager<object> _cache,IOrderService _order)
        {
            cacheManager = _cache;
            orderService = _order;
        }
        // GET: Order
        [HttpGet]
        public async Task<ActionResult> RenderOrderTable(string keyword, bool mineOnly = false, int pageNum = 1)
        {
            GetListResult<BusinessOrder> orderList;
            if (mineOnly)
            {
                orderList = await orderService.RetrieveOrdersByKeyword(keyword, UserId);
            }else
            {
                orderList = await orderService.RetrieveOrdersByKeyword(keyword, null);
            }
            if (orderList.Entities == null) {
                orderList.Entities = new List<BusinessOrder>();
            }
            return PageJson(orderList.Entities.OrderByDescending(o=>o.LastModifiedAt).ThenBy(o=>o.CommenceDate).ToPagedList(pageNum, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AssignOrderToSales(string salesId,string orderId)
        {
            return OriginJson(await orderService.AssignOrderToSales(salesId, orderId));
        }
        public async Task<ActionResult> OrderDetail(string id)
        {
            var result = await orderService.RetrieveOrderById(id);
            if (!result.Success) {
                return HttpNotFound();
            }
            OrderOrderDetailViewModel model = new OrderOrderDetailViewModel()
            {
                Order = result.Entity,
                Role = GetClaimValue(ClaimTypes.Role),
                validUser = result.Entity.SalesId == UserId || this.User.IsInRole(UserRole.Admin),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateOrder([Bind(Include = "Id, ItemId, ItemName, ProviderName, ContactName, ContactNumber, SalesId, SalesName, Price, Status, CommenceDate, FinishDate, OrderDetail")]BusinessOrder order)
        {
            if (ModelState.IsValid)
            {
                order.ModificatorId = UserId;
                order.ModificatorName = GetClaimValue("NickName");
                bool authorized = GetClaimValue(ClaimTypes.Role) == "管理员";
                return OriginJson((await orderService.UpdateOrderById(UserId, order, authorized)));
            }
            return ErrorJson("模型错误");
        }
    }
}