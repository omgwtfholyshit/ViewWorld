using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.ExtensionMethods;
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
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return (await Repo.GetAllAsync<Scenery>()).ManyToListResult();
            }else
            {
                var builder = Builders<Scenery>.Filter;
                FilterDefinition<Scenery> filter;
                if(keyword.Length == 1)
                {
                    filter = builder.Eq("Initial", keyword);
                }
                else
                {
                    filter = builder.In("Name", keyword) | builder.In("EnglishName", keyword) | builder.In("Address", keyword)
                        | builder.In("Publisher", keyword) | builder.In("Modificator", keyword);
                }
                return (await Repo.GetManyAsync(filter)).ManyToListResult();
            }
        }

        public async Task<Result> UpdateEntity(Scenery Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }

        public async Task<Result> UpdatePhotos(List<string> photoList)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UploadPhotos()
        {
            throw new NotImplementedException();
        }
    }
}
