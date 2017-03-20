using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Dal;
using ViewWorld.Core.ExtensionMethods;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Utils;

namespace ViewWorld.Services.Sceneries
{
    public class SceneryService : ISceneryService
    {
        private readonly IMongoDbRepository Repo;
        const string photoDirectory = "/Upload/Sceneries/";
        public SceneryService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
        }

        public async Task<Result> AddEntity(Scenery Entity)
        {
            Entity.Id = ObjectId.GenerateNewId().ToString();
            var result = await Repo.AddOneAsync(Entity);
            if (result.Success)
            {
                result.Message = Entity.Id;
            }
            return result;
        }


        public async Task<Result> DeleteEntityById(string id)
        {
            var photoFolder = PathHelper.MapPath(photoDirectory + id);
            if (Directory.Exists(photoFolder))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(photoFolder);
                try
                {
                    directoryInfo.Delete(true);
                }catch(Exception e)
                {
                    Tools.WriteLog("Scenery", "删除Scenery时清理其图片", e.Message);
                }
                
            }
                
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
            var prevEntity = await Repo.GetOneAsync<Scenery>(Entity.Id);
            if (prevEntity.Success)
            {
                prevEntity.Entity.Address = Entity.Address;
                prevEntity.Entity.Coordinate = Entity.Coordinate;
                prevEntity.Entity.EnglishName = Entity.EnglishName;
                prevEntity.Entity.Initial = Entity.Initial;
                prevEntity.Entity.Name = Entity.Name;
                prevEntity.Entity.Modificator = Entity.Modificator;
                prevEntity.Entity.RegionId = Entity.RegionId;
                prevEntity.Entity.LastUpdateAt = DateTime.Now;
                return await Repo.ReplaceOneAsync(Entity.Id, prevEntity.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该数据", Success = false };
        }

        public Task<Result> UploadPhoto(HttpFileCollectionBase files, string id)
        {
            var result = new Result() { ErrorCode = 300, Message = "", Success = false };
            HttpPostedFileBase file;
            try
            {
                for(int i = 0; i < files.AllKeys.Count(); i++)
                {
                    file = files[i];
                    string savePath = PathHelper.MapPath(photoDirectory + id + "/");
                    if (!ImageHelper.CheckImageByFileExtension(Path.GetExtension(file.FileName)))
                    {
                        result.Message += file.FileName + ',';
                    }
                    else
                    {
                        string filePath = savePath + Tools.GenerateId_M1() + Path.GetExtension(file.FileName).ToLower();
                        if (!Directory.Exists(savePath))
                            Directory.CreateDirectory(savePath);
                        file.SaveAs(filePath);
                    }
                }
                if (string.IsNullOrWhiteSpace(result.Message))
                {
                    result.Success = true;
                    result.ErrorCode = 200;
                }
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                return Task.FromResult(result);
            }
            
        }

        public Task<List<string>> ListPhotos(string id)
        {
            List<string> result = new List<string>();
            string photoPath = PathHelper.MapPath(photoDirectory + id + "/");
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            result.AddRange(Directory.EnumerateFiles(photoPath).ToVirtualPaths());
            return Task.FromResult(result);
        }

        public Task<Result> DeletePhotoByFileName(string sceneryId, string fileName)
        {
            var result = new Result() { ErrorCode = 300, Message = "", Success = false };
            string photoPath = PathHelper.MapPath(photoDirectory + sceneryId + "/" + fileName);
            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
                result.Success = true;
                result.ErrorCode = 200;
            }
            else
            {
                result.Message = "找不到指定文件";
            }
            return Task.FromResult(result);
        }
    }
}
