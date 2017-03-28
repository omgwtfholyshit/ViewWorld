using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Core.ExtensionMethods;

namespace ViewWorld.Services.Cities
{
    public class CityService : ICityService
    {
        private readonly IMongoDbRepository Repo;
        public CityService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
        }
        public async Task<Result> AddEntity(CityInfo Entity)
        {
            var result = new Result() { ErrorCode = 300, Message = "城市已存在", Success = false };
            var filter = Builders<CityInfo>.Filter.Eq("Name",Entity.Name);
            bool exist = (await Repo.GetOneAsync(filter)).Success;
            if (!exist)
            {
                result = await Repo.AddOneAsync(Entity);
            }
            return result;
        }

        public async Task<Result> DeleteEntityById(string id)
        {
            return await Repo.DeleteOneAsync<CityInfo>(id);
        }

        public async Task<GetListResult<CityInfo>> RetrieveEntitiesByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return (await Repo.GetAllAsync<CityInfo>()).ManyToListResult();

            var filterItem = "Name";
            if (keyword.Length == 1)
            {
                filterItem = "Initial";
                keyword = keyword.ToUpper();
            }
            var filter = Builders<CityInfo>.Filter.Eq(filterItem, keyword);
            return (await Repo.GetManyAsync(filter)).ManyToListResult();
        }

        public async Task<Result> UpdateEntity(CityInfo Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }
    }
}
