using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.ExtensionMethods;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.StartingPoints
{
    public class StartingPointService : IStartingPointService
    {
        private readonly IMongoDbRepository Repo;
        public StartingPointService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
        }
        public async Task<Result> AddEntity(StartingPoint Entity)
        {
            return await Repo.AddOneAsync(Entity);
        }

        public async Task<Result> DeleteEntityById(string id)
        {
            return await Repo.DeleteOneAsync<StartingPoint>(id);
        }

        public async Task<GetListResult<StartingPoint>> RetrieveEntitiesByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return (await Repo.GetAllAsync<StartingPoint>()).ManyToListResult();
            }
            else
            {
                var builder = Builders<StartingPoint>.Filter;
                keyword = keyword.ToUpper();
                FilterDefinition<StartingPoint> filter = builder.Where(point => point.Address.Contains(keyword) || point.ProviderName.Contains(keyword) || point.ProviderAlias.Contains(keyword));
                return (await Repo.GetManyAsync(filter)).ManyToListResult();
            }
        }

        public async Task<Result> UpdateEntity(StartingPoint Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }
    }
}
