using CacheManager.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Services.Cities;
using ViewWorld.Services.Regions;
using ViewWorld.Services.Sceneries;
using ViewWorld.Services.StartingPoints;
using ViewWorld.Services.Trips;

namespace ViewWorld.Controllers.Trip
{
    [Authorize(Roles = "管理员,销售")]
    public class TripController : BaseController
    {
        #region Constructor
        // GET: Trip
        readonly IRegionService regionService;
        readonly ISceneryService sceneryService;
        readonly ICityService cityService;
        readonly ITripService tripService;
        readonly IStartingPointService startingPointService;
        ICacheManager<object> cacheManager;
        public TripController(IRegionService _regionService, ISceneryService _sceneryService, ICityService _cityService, ITripService _tripService, IStartingPointService _startingPointService, ICacheManager<object> _cache)
        {
            regionService = _regionService;
            sceneryService = _sceneryService;
            cityService = _cityService;
            tripService = _tripService;
            startingPointService = _startingPointService;
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
                    RemoveOutputCacheItem(cachedMethods[0], "Trip");
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
                RemoveOutputCacheItem(cachedMethods[0], "Trip");
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
            if (result.Success)
            {
                RemoveOutputCacheItem(cachedMethods[0], "Trip");
            }
            return OriginJson(result);
        }
        public async Task<JsonResult> SearchRegions(string keyword)
        {
            return Json((await regionService.RetrieveEntitiesByKeyword(keyword)).Entities.Where(e => e.IsVisible).OrderBy(e => e.SortOrder).OrderBy(e => e.Initial));
        }
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Server,VaryByParam = "displaySubRegions",Duration =120)]
        public async Task<JsonResult> ListRegionsAPI(bool displaySubRegions = false)
        {
            GetManyResult<Region> result = await regionService.GetRegions(false, true);
            if (result.Success)
            {
                var entities = result.Entities.ToList();
                List<DropdownDataStruct> dropdownDataList = new List<DropdownDataStruct>(); 
                if (displaySubRegions)
                {
                    entities.ForEach(e =>
                    {
                        dropdownDataList.Add(new DropdownDataStruct { name = e.Name, value = e.Id, });
                        if (e.SubRegions.Count() > 0)
                        {
                            e.SubRegions.ForEach(sub =>
                            {
                                dropdownDataList.Add(new DropdownDataStruct { name = "----" + sub.Name, value = sub.Id, results=sub.Initial});
                            });
                        }
                    });
                }
                else
                {
                    entities.Insert(0, new Region { Id = "-1", Name = "无", EnglishName = "null" });
                    entities.ForEach(e =>
                    {
                        dropdownDataList.Add(new DropdownDataStruct { name = e.Name, value = e.Id,results= e.Initial });
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
        public async Task<JsonResult> UpdateScenery([Bind(Include = "Id,Name,EnglishName,Coordinate,Initial,Address,RegionId,ExtraCost,Description")] Scenery model)
        {
            if (ModelState.IsValid)
            {

                model.Modificator = GetClaimValue("NickName").ToUpper();
                var result = await sceneryService.UpdateEntity(model);
                if (result.Success)
                {
                    cacheManager.Remove(cachedMethods[1]);
                    return OriginJson(result);
                }
            }
            return ErrorJson("Model Invalid");
        }
        [HttpGet]
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
        public async Task<JsonResult> GetSceneryDescription(string sceneryId)
        {
            var result = await sceneryService.RetrieveEntitiesById(sceneryId);
            if (result.Success)
            {
                return Json(result.Entity.Description);
            }
            return ErrorJson(result.Message);
        }
        [HttpGet]
        public async Task<JsonResult> ListSceneryPhotos(string sceneryId)
        {
            var result = await sceneryService.ListPhotos(sceneryId);
            return Json(result);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteSceneryPhotoByFileName(string sceneryId,string fileName)
        {
            return OriginJson(await sceneryService.DeletePhotoByFileName(sceneryId, fileName));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadSceneryPhotos(string sceneryId)
        {
            Result result = await sceneryService.UploadPhoto(Request.Files,sceneryId);
            if (result.Success)
            {
                return SuccessJson();
            }
            return ErrorJson(result.Message);
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
            var searchResult = await sceneryService.RetrieveEntitiesByKeyword(result, keyword);
            if (searchResult.Success)
            {
                List<Scenery> sceneries = searchResult.Entities;
                return PartialView("~/Views/PartialViews/_PartialSceneryTable.cshtml", sceneries.OrderByDescending(s => s.LastUpdateAt).ThenByDescending(s => s.Popularity));
            }
            return null;
        }
        #endregion
        #region 城市管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddCity(CityInfo model)
        {
            var result = await cityService.AddEntity(model);
            if (result.Success)
                return OriginJson(result);
            return ErrorJson(result.Message);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditCity(CityInfo model)
        {

            if (ModelState.IsValid)
            {
                var result = await cityService.UpdateEntity(model);
                if (result.Success)
                    return OriginJson(result);
                return ErrorJson(result.Message);
            }
            return ErrorJson("参数错误");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteCity(string id)
        {

            if (ModelState.IsValid)
            {
                var result = await cityService.DeleteEntityById(id);
                if (result.Success)
                    return OriginJson(result);
                return ErrorJson(result.Message);
            }
            return ErrorJson("参数错误");
        }
        [HttpGet]
        public async Task<JsonResult> SearchCityByKeyword(string keyword)
        {
            var result = await cityService.RetrieveEntitiesByKeyword(keyword);
            return Json(result.Entities.OrderBy(e=>e.Initial));
        }
        #endregion
        #region 出发地管理
        [HttpGet]
        public async Task<JsonResult> ListStartingPoints(string keyword)
        {
            var result = await startingPointService.RetrieveEntitiesByKeyword(keyword);
            if (result.Success)
            {
                return OriginJson(result);
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        [HttpGet]
        public async Task<JsonResult> ListStartingPointsAPI(string keyword)
        {
            var result = await startingPointService.RetrieveEntitiesByKeyword(keyword);
            if (result.Success)
            {
                return Json(result.Entities.Select(point => new { name = point.ProviderName + "|" + point.Address, value = point.Id }));
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddStartingPoint(StartingPoint point)
        {
            point.UpdatedBy = GetClaimValue("NickName");
            return OriginJson(await startingPointService.AddEntity(point));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateStartingPoint(StartingPoint point)
        {
            point.ModifiedDate = DateTime.Now;
            point.UpdatedBy = GetClaimValue("NickName");
            return OriginJson(await startingPointService.UpdateEntity(point));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteStartingPointById(string pointId)
        {
            return OriginJson(await startingPointService.DeleteEntityById(pointId));
        }

        #endregion
        #region 行程管理
        [HttpGet]
        public JsonResult ListTripTypeAPI()
        {
            var list = tripService.ListTripType();
            var data = new
            {
                success = true,
                results = list
            };
            return OriginJson(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddTripArrangement(TripArrangement entity)
        {
            entity.Publisher = GetClaimValue("NickName").ToUpper();
            entity.PublisherId = UserId;
            entity.PublishedAt = DateTime.Now;
            return OriginJson(await tripService.AddEntity(entity));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateTripArrangement(TripArrangement entity)
        {
            return OriginJson(await tripService.UpdateEntity(entity));
        }
        [HttpGet]
        public async Task<JsonResult> GetTripArrangementById(string tripId)
        {
            return OriginJson(await tripService.RetrieveTripArrangementById(tripId));
        }
        [HttpGet]
        public async Task<ActionResult> RenderTripArrangementByKeyword(string keyword)
        {
            var result = await tripService.RetrieveEntitiesByKeyword(keyword);
            return PartialView("~/Views/PartialViews/_PartialTripTable.cshtml", result.Entities.OrderByDescending(trip => trip.PublishedAt));
        }
        [HttpGet]
        public async Task<JsonResult> RetrieveTripArrangementByProductId(string productId)
        {
            return OriginJson(await tripService.RetrieveTripArrangementByProductId(productId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateTripPartial(string tripId,string data,TripTypes.TripInfoType type)
        {
            var result = new Result() { ErrorCode = 300, Success = false, Message = "" };
            var serializer = new JavaScriptSerializer();
            try
            {
                switch (type)
                {
                    case TripTypes.TripInfoType.通用信息:
                        result = await tripService.UpdateTripPartial(tripId, serializer.Deserialize<CommonInfo>(data));
                        break;
                    case TripTypes.TripInfoType.产品概要:
                        result = await tripService.UpdateTripPartial(tripId, serializer.Deserialize<ProductInfo>(data));
                        break;
                    case TripTypes.TripInfoType.单日行程:
                        result = await tripService.UpdateTripPartial(tripId, serializer.Deserialize<List<Schedule>>(data));
                        break;
                    case TripTypes.TripInfoType.发团属性:
                        result = await tripService.UpdateTripPartial(tripId, serializer.Deserialize<TripProperty>(data));
                        break;
                    case TripTypes.TripInfoType.发团计划:
                        result = await tripService.UpdateTripPartial(tripId, serializer.Deserialize<List<TripPlan>>(data));
                        break;
                    default:
                        result.Message = "行程类型错误";
                        break;
                }
                return OriginJson(result);
            }catch(Exception e)
            {
                result.Message = e.Message;
                return OriginJson(result);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadTripArrangementPhoto()
        {
            Result result = await tripService.UploadPhoto(Request);
            if (result.Success)
            {
                return Json(result.Message);
            }
            return ErrorJson(result.Message);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ToggleTripArrangement(string tripId)
        {
            return OriginJson(await tripService.ToggleTripArrangement(tripId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DisplayTripOnFrontPage(string tripId)
        {
            return OriginJson(await tripService.DisplayTripOnFrontPage(tripId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CopyTripArrangementById(string tripId)
        {
            return OriginJson(await tripService.CopyTripArrangement(tripId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteTripArrangementById(string tripId)
        {
            return OriginJson(await tripService.DeleteEntityById(tripId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SetFrontCoverById(string tripId, string photoId)
        {
            return OriginJson(await tripService.SetFrontCover(tripId, photoId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeletePhotoById(string tripId,string photoId)
        {
            return OriginJson(await tripService.DeletePhotoById(tripId, photoId));
        }
        #endregion
    }
}