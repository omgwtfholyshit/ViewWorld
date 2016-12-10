using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
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
        public async Task<Result> AddProvider(Provider model)
        {
            return await Repo.AddOneAsync(model);
        }

        public async Task<Result> DeleteProvider(string id)
        {
            var update = Builders<Provider>.Update.Set("IsArchived", true).CurrentDate("ModifiedDate");
            return await Repo.UpdateOneAsync(id, update);
        }

        public async Task<Result> EditProvider(Provider model)
        {
            return await Repo.ReplaceOneAsync(model.Id, model);
        }

        public async Task<GetManyResult<Provider>> ListProviders()
        {
            return await Repo.GetAllAsync<Provider>();
        }
    }
}
