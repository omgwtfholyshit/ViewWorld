using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.ExtensionMethods;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Utils;

namespace ViewWorld.Services.Regions
{
    public class RegionService : IRegionService
    {
        private readonly IMongoDbRepository Repo;
        public RegionService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
        }
        public async Task<Result> AddEntity(Region Entity)
        {
            try
            {
                if (Entity.IsSubRegion && Entity.ParentRegionId != "-1")
                {
                    var parent = await Repo.GetOneAsync<Region>(Entity.ParentRegionId);
                    Entity.Id = ObjectId.GenerateNewId().ToString();
                    parent.Entity.SubRegions.Add(Entity);
                    return await UpdateEntity(parent.Entity);
                }
                else
                {
                    return await Repo.AddOneAsync(Entity);
                }
            }
            catch (Exception e)
            {
                return new Result { ErrorCode = 300, Message = e.Message, Success = false };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId">This region's parentId, in case to update it.</param>
        /// <param name="id">This region's Id</param>
        /// <param name="destId">Target region Id. It needs to be a main region.</param>
        /// <returns></returns>
        public async Task<Result> ChangeRegion(string parentId, string id, string destId)
        {
            Result result = new Result { ErrorCode = 300, Message = "", Success = false };
            string[] ids = new string[2] { parentId, destId };
            var regions = await Repo.GetManyAsync<Region>(ids);
            if (regions.Success && regions.Entities.Count() == 2)
            {
                try
                {
                    var parent = regions.Entities.ElementAt(0);
                    var dest = regions.Entities.ElementAt(1);
                    if (!parent.IsSubRegion && !dest.IsSubRegion)
                    {
                        var child = parent.SubRegions.Find(r => r.Id == id);
                        dest.SubRegions.Add(child);
                        await UpdateEntity(dest);
                        parent.SubRegions.Remove(child);
                        await UpdateEntity(parent);
                        result.Success = true;
                        result.ErrorCode = 200;
                    }
                    else
                    {
                        result.Message = "不能移动到子区域";
                    }


                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                }

            }
            else
            {
                result.Message = "找不到该区域";
            }
            return result;
        }

        public async Task<Result> DeleteRegion(string id, string parentId)
        {
            var result = new Result { ErrorCode = 300, Message = "", Success = false };
            try
            {
                if (string.IsNullOrWhiteSpace(parentId) || parentId == "-1")
                {
                    return await DeleteEntityById(id);
                }
                else
                {
                    var parent = await Repo.GetOneAsync<Region>(parentId);
                    var deleteCount = parent.Entity.SubRegions.RemoveAll(r => r.Id == id);
                    if (deleteCount > 0)
                    {
                        if (deleteCount != 1)
                        {
                            Tools.WriteLog("Region", "删除", string.Format("此次删除{0}条记录，数据库可能有异常，请检查", deleteCount));
                        }
                        return await UpdateEntity(parent.Entity);
                    }
                    result.Message = "找不到该区域";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                return result;
            }
        }

        public async Task<GetManyResult<Region>> GetRegions(bool VisibileOnly = true, bool MainRegionOnly = false)
        {
            FilterDefinition<Region> filter;
            var builder = Builders<Region>.Filter;
            if (VisibileOnly && MainRegionOnly)
            {
                filter = builder.Eq("IsVisible", true) & builder.Eq("IsSubRegion", false);
                return await Repo.GetManyAsync<Region>(filter);
            }
            else if (!VisibileOnly && MainRegionOnly)
            {
                filter = builder.Eq("IsSubRegion", false);
                return await Repo.GetManyAsync<Region>(filter);
            }
            else
            {
                return await Repo.GetAllAsync<Region>();
            }

        }

        public async Task<GetListResult<Region>> RetrieveEntitiesByKeyword(string keyword)
        {
            var result = new GetListResult<Region> { Success = false, ErrorCode = 300, Message = "", Entities = null };
            var regions = await GetRegions(false);
            
            if (regions.Success)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return regions.ManyToListResult();

                var regionList = new List<Region>();
                keyword = keyword.ToUpper();
                foreach (var region in regions.Entities)
                {
                    if (IsRegionMatch(region, keyword))
                    {
                        if (!region.IsSubRegion && region.SubRegions != null)
                        {
                            region.SubRegions = region.SubRegions.FindAll(s => s.Name.Contains(keyword) || s.EnglishName.ToUpper().Contains(keyword));
                        }
                        regionList.Add(region);
                    }
                }
                result.Success = true;
                result.ErrorCode = 200;
                result.Entities = regionList;
            }
            return result;
        }
        public async Task<GetListResult<Region>> RetrieveEntitiesByKeyword(string keyword,GetManyResult<Region> regions)
        {
            var result = new GetListResult<Region> { Success = false, ErrorCode = 300, Message = "", Entities = null };
            var regionList = new List<Region>();
            if (regions.Success)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return await Task.FromResult(regions.ManyToListResult());

                keyword = keyword.ToUpper();
                foreach (var region in regions.Entities)
                {
                    if (IsRegionMatch(region, keyword))
                    {
                        if (!region.IsSubRegion && region.SubRegions != null)
                        {
                            region.SubRegions = region.SubRegions.FindAll(s => s.Name.Contains(keyword) || s.EnglishName.ToUpper().Contains(keyword));
                        }
                        regionList.Add(region);
                    }
                }
                result.Success = true;
                result.ErrorCode = 200;
                result.Entities = regionList;
            }
            return await Task.FromResult(result);
        }

        bool IsRegionMatch(Region region, string keyword)
        {
            if (keyword.Length == 1 && !Tools.isChineseLetter(keyword))
            {
                return region.Initial.Contains(keyword) ||
                    (!region.IsSubRegion && region.SubRegions != null &&
                    region.SubRegions.Where(r => r.Initial.Contains(keyword)).Count() > 0);
            }
            else
            {
                return (region.Name.Contains(keyword) || region.EnglishName.ToUpper().Contains(keyword)) ||
                    (!region.IsSubRegion && region.SubRegions != null &&
                    (region.SubRegions.Where(r => r.Name.Contains(keyword)).Count() > 0 ||
                    region.SubRegions.Where(r => r.EnglishName.ToUpper().Contains(keyword)).Count() > 0));
            }
        }

        public async Task<Result> UpdateEntity(Region Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }
        public async Task<Result> UpdateRegion(string id, Region model)
        {
            model.Id = id;
            return await Repo.ReplaceOneAsync(id, model);
        }

        public async Task<Result> UpdateSubRegion(Region model)
        {
            var parent = (await Repo.GetOneAsync<Region>(model.ParentRegionId)).Entity;
            var modelIndex = parent.SubRegions.FindIndex(r => r.Id == model.Id);
            parent.SubRegions[modelIndex] = model;
            return await UpdateEntity(parent);
        }
        public Task<Result> DeleteEntityById(string id)
        {
            return Repo.DeleteOneAsync<Region>(id);
        }
    }
}
