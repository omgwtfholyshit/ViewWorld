using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Sceneries
{
    public class SceneryService : ISceneryService
    {
        private readonly IMongoDbRepository Repo;
        public SceneryService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
        }

        public async Task<Result> AddEntity(Scenery Entity)
        {
            return await Repo.AddOneAsync(Entity);
        }


        public async Task<Result> DeleteEntityById(string id)
        {
            return await Repo.DeleteOneAsync<Scenery>(id);
        }

        public async Task<GetListResult<Scenery>> RetrieveEntitiesByKeyword(string keyword)
        {
            var result = new GetListResult<Scenery>();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var getAllResult = await Repo.GetAllAsync<Scenery>();
                result.Success = getAllResult.Success;
                result.Message = getAllResult.Message;
                if (getAllResult.Success)
                {
                    result.Entities = getAllResult.Entities.ToList();
                }
            }else
            {
                throw new NotImplementedException();
            }
            return null;
        }

        public async Task<Result> UpdateEntity(Scenery Entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdatePhotos(List<string> photoList)
        {
            throw new NotImplementedException();
        }

    }
}
