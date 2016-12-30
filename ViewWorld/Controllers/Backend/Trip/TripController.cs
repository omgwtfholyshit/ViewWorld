using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Driver;
using ViewWorld.Models.Managers;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Services.Regions;
using CacheManager.Core;
using ViewWorld.Services.Sceneries;
using ViewWorld.Core.Models;

namespace ViewWorld.Controllers.Trip
{
    public class TripController : BaseController
    {
        #region Constructor
        // GET: Trip
        readonly IRegionService regionService;
        readonly ISceneryService sceneryService;
        ICacheManager<object> cacheManager;
        public TripController(IRegionService _regionService,ISceneryService _sceneryService, ICacheManager<object> _cache)
        {
            regionService = _regionService;
            sceneryService = _sceneryService;
            cacheManager = _cache;
        }
        #endregion
        static string[] cachedMethods = new string[]{"ListRegionsAPI", "ListSceneriesAPI" };
        #region 区域管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddRegion(Region model)
        {
            if (ModelState.IsValid)
            {
                if(model.ParentRegionId != "-1" && model.Name != "无")
                {
                    model.IsSubRegion = true;
                }
                var result = await regionService.AddEntity(model);
                if (result.Success)
                {
                    cacheManager.Remove(cachedMethods[0]);
                }
                return OriginJson(result);
            }
            return ErrorJson("参数错误");
        }
        public async Task<JsonResult> DeleteRegion(string id,string parentId)
        {
            var result = await regionService.DeleteRegion(id,parentId);
            if (result.Success)
            {
                cacheManager.Remove(cachedMethods[0]);
            }
            return OriginJson(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateRegion(Region model,string prevParentId)
        {
            Result result = new Result
            {
                ErrorCode = 300,
                Message = "",
                Success = false,
            };
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    
                    if(string.IsNullOrWhiteSpace(prevParentId) || prevParentId == "-1")
                    {
                        if(model.ParentRegionId == prevParentId)
                        {
                            result = await regionService.UpdateEntity(model);
                        }else
                        {
                            result.Message = "主区域不能移动";
                        }

                    }else
                    {
                        if(model.ParentRegionId != prevParentId)
                        {
                            result = await regionService.ChangeRegion(prevParentId, model.Id, model.ParentRegionId);
                        }else
                        {
                            result = await regionService.UpdateSubRegion(model);
                        }
                    }
                }
            }
            return OriginJson(result);
        }
        public async Task<JsonResult> SearchRegions(string keyword)
        {
            return Json((await regionService.RetrieveEntitiesByKeyword(keyword)).Entities.Where(e => e.IsVisible).OrderBy(e => e.SortOrder).OrderBy(e => e.Initial));
        }
        public async Task<JsonResult> ListRegionsAPI(bool displaySubRegions = false)
        {
            GetManyResult<Region> result;
            result = cacheManager.Get(cachedMethods[0]) as GetManyResult<Region>;
            if (result == null || !result.Success)
            {
                result = await regionService.GetRegions(false, true);
                cacheManager.Add(cachedMethods[0], result);
            }
            if (result.Success)
            {
                var entities = result.Entities.ToList();
                List<DropdownDataStruct> dropdownDataList = new List<DropdownDataStruct>(); ;
                if (displaySubRegions)
                {
                    entities.ForEach(e =>
                    {
                        dropdownDataList.Add(new DropdownDataStruct { name = e.Name, values = e.Id, });
                        if (e.SubRegions.Count() > 0)
                        {
                            e.SubRegions.ForEach(sub =>
                            {
                                dropdownDataList.Add(new DropdownDataStruct { name = "  " + sub.Name, values = sub.Id, });
                            });
                        }
                    });
                }
                else
                {
                    entities.Insert(0, new Region { Id = "-1", Name = "无", EnglishName = "null" });
                    entities.ForEach(e =>
                    {
                        dropdownDataList.Add(new DropdownDataStruct { name = e.Name, values = e.Id, });
                    });
                }
                var data = new
                {
                    success = result.Success,
                    results = dropdownDataList
                };
                return OriginJson(data);
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        public async Task<JsonResult> ListSubRegionsApi()
        {
            var result = (await regionService.GetRegions(false, false));
            if (result.Success)
            {
                List<Region> subRegions = new List<Region>();
                result.Entities.ToList().ForEach(r =>
                {
                    if (r.SubRegions.Count() > 0)
                    {
                        subRegions.AddRange(r.SubRegions);
                    }
                });
                var data = new
                {
                    success = result.Success,
                    results = subRegions.Select(r => new { name = r.Name, value = r.Id })
                };
                return OriginJson(data);
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        public async Task<JsonResult> ChangeRegion(string parentId, string id, string destId)
        {
            var result = await regionService.ChangeRegion(parentId, id, destId);
            if (result.Success)
            {
                RemoveOutputCacheItem(cachedMethods[0], "Trip");
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<ActionResult> _PartialRegionTable(string keyword)
        {
            List<Region> regions = (await regionService.RetrieveEntitiesByKeyword(keyword)).Entities;
            
            return PartialView("~/Views/PartialViews/_PartialRegionTable.cshtml", regions.Where(e => e.IsVisible).OrderBy(e => e.SortOrder).OrderBy(e => e.Initial));
        }
        #endregion
        #region 景点管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddScenery(Scenery model)
        {
            if (ModelState.IsValid)
            {
                var result = await sceneryService.AddEntity(model);
                if (result.Success)
                {
                    cacheManager.Remove(cachedMethods[1]);
                }
                return OriginJson(result);
            }
            return ErrorJson("参数错误");
        }
        public async Task<JsonResult> DeleteScenery(string id)
        {
            var result = await sceneryService.DeleteEntityById(id);
            if (result.Success)
            {
                cacheManager.Remove(cachedMethods[1]);
            }
            return OriginJson(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateScenery(Scenery model)
        {
            return OriginJson(await sceneryService.UpdateEntity(model));
        }
        public async Task<JsonResult> ListSceneriesAPI()
        {
            GetListResult<Scenery> result;
            result = cacheManager.Get(cachedMethods[1]) as GetListResult<Scenery>;
            if (result == null || !result.Success)
            {
                result = await sceneryService.RetrieveEntitiesByKeyword("");
                cacheManager.Add(cachedMethods[1], result);
            }
            if (result.Success)
            {
                var entities = result.Entities.ToList();
                entities.Insert(0, new Scenery { Id = "-1", Name = "无", EnglishName = "null" });
                var data = new
                {
                    success = result.Success,
                    results = entities.Select(r => new { name = r.Name, value = r.Id })
                };
                return OriginJson(data);
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        [HttpGet]
        public async Task<ActionResult> _PartialSceneryTable(string keyword)
        {
            GetListResult<Scenery> result;
            result = cacheManager.Get(cachedMethods[1]) as GetListResult<Scenery>;
            if (result == null || !result.Success)
            {
                result = await sceneryService.RetrieveEntitiesByKeyword("");
                cacheManager.Add(cachedMethods[1], result);
            }
            List<Scenery> sceneries = (await sceneryService.RetrieveEntitiesByKeyword(result, keyword)).Entities;

            return PartialView("~/Views/PartialViews/_PartialSceneryTable.cshtml", sceneries.OrderByDescending(s=>s.Popularity));
        }
        #endregion
    }
}