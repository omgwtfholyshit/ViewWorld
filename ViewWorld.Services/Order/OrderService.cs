using CacheManager.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.BusinessModels;

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
                result = await Repo.AddOneAsync(Entity);
            }
            return result;
        }

        public virtual async Task<Result> DeleteEntityById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<GetListResult<BusinessOrder>> RetrieveEntitiesByKeyword(string keyword)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<Result> UpdateEntity(BusinessOrder Entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckIfOrderExist(string userId,string itemId, ProductType type, bool unexpiredOnly = true)
        {
            var builder = Builders<BusinessOrder>.Filter;
            var filter = builder.Where(o => o.UserId == userId && o.ItemId == itemId && o.Type == type);
            if (unexpiredOnly)
            {
                filter = builder.And(filter, builder.Where(o => o.Status != OrderStatus.Created));
            }
            var result = await Repo.GetOneAsync(filter);
            return result.Success;
        }
    }
}
