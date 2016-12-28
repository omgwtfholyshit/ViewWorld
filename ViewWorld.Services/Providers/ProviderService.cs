using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.ExtensionMethods;
using ViewWorld.Core.Models.ProviderModels;

namespace ViewWorld.Services.Providers
{
    public class ProviderService : IProviderService
    {
        readonly IMongoDbRepository Repo;
        public ProviderService(IMongoDbRepository _repo)
        {
            this.Repo = _repo;
        }

        public async Task<Result> AddEntity(Provider Entity)
        {
            return await Repo.AddOneAsync(Entity);
        }

        public async Task<Result> DeleteEntityById(string id)
        {
            var update = Builders<Provider>.Update.Set("IsArchived", true).CurrentDate("ModifiedDate");
            return await Repo.UpdateOneAsync(id, update);
        }

        public async Task<Result> UpdateEntity(Provider Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }

        public async Task<GetManyResult<Provider>> ListProviders()
        {
            return await Repo.GetAllAsync<Provider>();
        }

        public async Task<GetListResult<Provider>> RetrieveEntitiesByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return (await ListProviders()).ManyToListResult();
            var builder = Builders<Provider>.Filter;
            var filter = builder.Or(builder.In("Name", keyword), builder.In("ContactName", keyword),
                builder.In("Phone", keyword), builder.In("Email", keyword));
            return (await Repo.GetManyAsync(filter)).ManyToListResult();
            
                
        }

    }
}
