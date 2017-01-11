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

        public async Task<GetOneResult<Scenery>> RetrieveEntitiesById(GetListResult<Scenery> cachedData, string id)
        {
            GetOneResult<Scenery> result = new GetOneResult<Scenery>() { Entity = null, ErrorCode = 300, Message = "", Success = false };
            if (string.IsNullOrWhiteSpace(id))
            {
                result.Message = "id不能为空";
                return result;
            }
            
            if(cachedData != null)
            {
                var entity =  cachedData.Entities.FirstOrDefault(s => s.Id == id);
                if(entity!=null)
                {
                    result.Success = true;
                    result.Entity = entity;
                    result.ErrorCode = 200;
                }
                else
                {
                    result.Message = "找不到该景点";
                }
            }
            else
            {
                result = await Repo.GetOneAsync<Scenery>(id);
            }
            return result;
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
                keyword = keyword.ToUpper();
                if(keyword.Length == 1)
                {
                    filter = builder.Eq("Initial", keyword);
                }
                else
                {
                    filter = builder.Where(obj => obj.Name.Contains(keyword) || obj.EnglishName.Contains(keyword) ||
                    obj.Address.ToUpper().Contains(keyword) || obj.Publisher.ToUpper().Contains(keyword) || obj.Modificator.ToUpper().Contains(keyword));
                }
                return (await Repo.GetManyAsync(filter)).ManyToListResult();
            }
        }

        public async Task<GetListResult<Scenery>> RetrieveEntitiesByKeyword(GetListResult<Scenery> cachedData, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return cachedData;
            }else
            {
                var filteredResult = new GetListResult<Scenery> { Success = true, Message = "" };
                keyword = keyword.ToUpper();
                if (keyword.Length == 1)
                {
                    filteredResult.Entities = cachedData.Entities.FindAll(e => e.Initial == keyword);
                }else
                {
                    filteredResult.Entities = cachedData.Entities.FindAll(obj => obj.Name.Contains(keyword) || obj.EnglishName.Contains(keyword) ||
                    obj.Address.ToUpper().Contains(keyword) || obj.Publisher.ToUpper().Contains(keyword) || obj.Modificator.ToUpper().Contains(keyword));
                }

                return await Task.FromResult(filteredResult);
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
