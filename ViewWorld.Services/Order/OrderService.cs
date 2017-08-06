using CacheManager.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.ExtensionMethods;
using ViewWorld.Core.Models.BusinessModels;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IMongoDbRepository Repo;
        ICacheManager<object> cacheManager;
        public OrderService(IMongoDbRepository _Repo, ICacheManager<object> _cacheManager)
        {
            this.Repo = _Repo;
            this.cacheManager = _cacheManager;
        }
        public virtual async Task<Result> AddEntity(BusinessOrder Entity)
        {
            var result = new Result() { ErrorCode = 300, Message = "订单已提交", Success = false };
            var existed = await CheckIfOrderExist(Entity.UserId, Entity.ItemId, Entity.Type);
            if (!existed)
            {
                var uuid = ObjectId.GenerateNewId();
                Entity.Id = uuid.ToString();
                Entity.Status = OrderStatus.新创建订单;
                Entity.OrderId = Utils.Tools.GenerateId_M3(uuid.Increment);
                result = await Repo.AddOneAsync(Entity);
            }
            return result;
        }

        public virtual async Task<Result> DeleteEntityById(string id)
        {
            var result = await Repo.GetOneAsync<BusinessOrder>(id);
            if (result.Entity.Status == OrderStatus.新创建订单)
            {
                result.Entity.Status = OrderStatus.订单已删除;
                result.Entity.LastModifiedAt = DateTime.Now;
                result.Entity.ModificatorId = result.Entity.UserId;
            }
            return await UpdateEntity(result.Entity);
        }

        public virtual async Task<GetListResult<BusinessOrder>> RetrieveEntitiesByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return (await Repo.GetAllAsync<BusinessOrder>()).ManyToListResult();
            }
            var builder = Builders<BusinessOrder>.Filter;
            keyword = keyword.ToUpper();
            var filter = builder.Where(order => order.ItemName.Contains(keyword) || order.ContactNumber == keyword || order.SalesName == keyword || order.OrderId == keyword);
            return (await Repo.GetManyAsync(filter)).ManyToListResult();
        }
        public virtual async Task<Result> UpdateEntity(BusinessOrder Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }
        public async Task<Result> UpdateOrderById(string salesId, BusinessOrder order, bool authorized = false)
        {
            var updateResult = new Result();
            var result = await Repo.GetOneAsync<BusinessOrder>(order.Id);
            if (result.Success)
            {
                //只有管理员才能更换负责人
                if (result.Entity.SalesId != order.SalesId)
                {
                    if (!authorized)
                    {
                        updateResult.Message = "您没有权限执行该操作";
                        return updateResult;
                    } else
                    {
                        result.Entity.SalesId = order.SalesId;
                        result.Entity.SalesName = order.SalesName;
                    }
                }

                if (result.Entity.SalesId == salesId || authorized)
                {
                    result.Entity.ContactName = order.ContactName;
                    result.Entity.ContactNumber = order.ContactNumber;
                    result.Entity.Price = order.Price;
                    result.Entity.Status = order.Status;
                    result.Entity.OrderDetail = order.OrderDetail;
                    result.Entity.LastModifiedAt = DateTime.Now;
                    result.Entity.ModificatorId = order.ModificatorId;
                    result.Entity.ModificatorName = order.ModificatorName;
                    result.Entity.ProviderName = order.ProviderName;
                    result.Entity.CommenceDate = order.CommenceDate;
                    result.Entity.FinishDate = order.FinishDate;
                    result.Entity.ItemId = order.ItemId;
                    result.Entity.ItemName = order.ItemName;
                    updateResult = await UpdateEntity(result.Entity);
                } else
                {
                    updateResult.Message = "您没有权限执行该操作";
                }

            }
            else
            {
                updateResult.Message = "找不到该订单";
            }
            return updateResult;
        }

        public async Task<bool> CheckIfOrderExist(string userId, string itemId, ProductType type, bool unexpiredOnly = true)
        {
            var builder = Builders<BusinessOrder>.Filter;
            var filter = builder.Where(o => o.UserId == userId && o.ItemId == itemId && o.Type == type);
            if (unexpiredOnly)
            {
                filter = builder.And(filter, builder.Where(o => o.Status == OrderStatus.新创建订单));
            }
            var result = await Repo.GetManyAsync(filter);
            return !(result.Entities.Count() == 0);
        }

        public Task<GetOneResult<BusinessOrder>> RetrieveOrderById(string id)
        {
            return Repo.GetOneAsync<BusinessOrder>(id);
        }

        public async Task<GetListResult<BusinessOrder>> RetrieveOrdersByKeyword(string keyword, string salesId)
        {
            var builder = Builders<BusinessOrder>.Filter;
            FilterDefinition<BusinessOrder> filter = builder.Where(order => order.Status != OrderStatus.订单已删除);
            if (!string.IsNullOrWhiteSpace(salesId))
            {
                filter = builder.And(filter, builder.Eq("SalesId", salesId));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToUpper();
                filter = builder.And(filter, builder.Where(order => order.ItemName.Contains(keyword) || order.ContactNumber == keyword || order.SalesName == keyword || order.OrderId == keyword));
            }
            return (await Repo.GetManyAsync(filter)).ManyToListResult();
        }

        public async Task<Result> AssignOrderToSales(string salesId, string orderId)
        {
            var result = new Result();
            ViewWorldPrincipal salePrincipal = new ViewWorldPrincipal(salesId);
            ViewWorldIdentity saleIdentity = salePrincipal.Identity as ViewWorldIdentity;
            if (salePrincipal.IsInRole("销售")||salePrincipal.IsInRole("管理员"))
            {
                var order = await Repo.GetOneAsync<BusinessOrder>(orderId);
                if (order.Success)
                {
                    if (string.IsNullOrEmpty(order.Entity.SalesId))
                    {
                        order.Entity.SalesId = salesId;
                        order.Entity.SalesName = saleIdentity.NickName;
                        result = await UpdateEntity(order.Entity);
                        result.Message = saleIdentity.NickName;
                    }
                    else
                    {
                        if(order.Entity.SalesId == salesId)
                        {
                            result.Message = "这个单子已经被你抢到啦！";
                        }
                        else
                        {
                            result.Message = "这个单子已经被别人抢啦！";
                        }
                    }
                    
                }
            }else
            {
                result.Message = "您没有权限执行该操作";
            }
            return result;
        }

        public async Task<Result> CreatePriceGapOrder(string orderId, double price , string salesId,bool authorized)
        {
            var result = await Repo.GetOneAsync<BusinessOrder>(orderId);
            if (result.Success)
            {
                if(result.Entity.SalesId == salesId || authorized)
                {
                    var GapOrder = new BusinessOrder()
                    {
                        CommenceDate = result.Entity.CommenceDate,
                        ContactName = result.Entity.ContactName,
                        ContactNumber = result.Entity.ContactNumber,
                        CurrencyType = result.Entity.CurrencyType,
                        FinishDate = result.Entity.FinishDate,
                        ItemId = result.Entity.ItemId,
                        ItemName = result.Entity.ItemName,
                        LastModifiedAt = DateTime.Now,
                        ModificatorId = result.Entity.ModificatorId,
                        ModificatorName = result.Entity.ModificatorId,
                        OrderDetail = result.Entity.OrderDetail,
                        OrderedAt = DateTime.Now,
                        OrderId = result.Entity.OrderId,
                        Price = price,
                        ProviderName = result.Entity.ProviderName,
                        SalesId = result.Entity.SalesId,
                        SalesName = result.Entity.SalesName,
                        Status = OrderStatus.行程已确认,
                        Type = ProductType.补差价订单,
                        UserId = result.Entity.UserId
                    };
                    var insertResult = await Repo.AddOneAsync(GapOrder);
                    result.Success = insertResult.Success;
                    result.Message = insertResult.Message;
                    result.ErrorCode = insertResult.ErrorCode;
                }
                else
                {
                    result.Success = false;
                    result.Message = "您没有权限执行该操作";
                    result.ErrorCode = 401;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "找不到该订单";
                result.ErrorCode = 404;
            }
            return result;

        }

        public Task<long> CountOrders(string userId, bool deletedOrders = false)
        {
            var builder = Builders<BusinessOrder>.Filter;
            FilterDefinition<BusinessOrder> filter = builder.Empty;
            if (!deletedOrders)
            {
                filter = builder.Where(o => o.Status != OrderStatus.订单已删除);
            }
            return Repo.CountAsync(filter);
        }
    }
}
