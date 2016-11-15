using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using MongoDB.Driver;
using ViewWorld.Models;
using ViewWorld.Models.Managers;
using ViewWorld.Models.Trip;
using System.Threading.Tasks;
using ViewWorld.Utils.ViewModels;

namespace ViewWorld.Controllers.Trip
{
    public class TripController : BaseController
    {
        #region Constructor
        // GET: Trip
        private TripManager tripManager;
        public TripController(TripManager _tripManager)
        {
            tripManager = _tripManager;
        }
        public TripController() : this(new TripManager())
        {

        }
        #endregion
        static string[] cachedMethods = new string[]{"ListRegionsAPI"};
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
                var result = await tripManager.AddRegion(model);
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
            var result = await tripManager.DeleteRegion(id,parentId);
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
                            result = await tripManager.UpdateRegion(model);
                        }else
                        {
                            result.Message = "主区域不能移动";
                        }
                        

                    }else
                    {
                        if(model.ParentRegionId != prevParentId)
                        {
                            result = await tripManager.ChangeRegion(prevParentId, model.Id, model.ParentRegionId);
                        }else
                        {
                            result = await tripManager.UpdateSubRegion(model);
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
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(await tripManager.GetRegions());
            }
            return Json(await tripManager.SearchRegions(keyword));
        }
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Server, Duration = 1200)]
        public async Task<JsonResult> ListRegionsAPI()
        {
            var result = await tripManager.GetRegions(false, true);
            if (result.Success)
            {
                var entities = result.Entities.ToList();
                entities.Insert(0, new Region { Id = "-1", Name = "无", EnglishName = "null" });
                var data = new
                {
                    success = result.Success,
                    results = entities.Select(r => new { name = r.Name, value = r.Id })
                };
                return OriginJson(data);
            }
            return ErrorJson("服务器内部错误，请稍后再试");
        }
        public async Task<JsonResult> ListSubRegionsApi()
        {
            var result = (await tripManager.GetRegions(false, false));
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
            var result = await tripManager.ChangeRegion(parentId, id, destId);
            if (result.Success)
            {
                RemoveOutputCacheItem(cachedMethods[0], "Trip");
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<ActionResult> _PartialRegionTable(string keyword)
        {
            List<Region> regions;
            if (string.IsNullOrEmpty(keyword))
            {
                regions = (await tripManager.GetRegions()).Entities.ToList();
                
            }else
            {
                regions = (await tripManager.SearchRegions(keyword)).Entities.ToList();
            }
            return PartialView("~/Views/PartialViews/_PartialRegionTable.cshtml", regions.OrderBy(r => r.SortOrder));
        }
        #endregion
    }
}