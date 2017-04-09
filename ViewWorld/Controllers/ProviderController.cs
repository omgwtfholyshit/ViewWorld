using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver;
using ViewWorld.Core.Models.ProviderModels;
using ViewWorld.Services.Providers;
using CacheManager.Core;
using ViewWorld.Core.Dal;
using System.Linq;

namespace ViewWorld.Controllers
{
    public class ProviderController : BaseController
    {
        readonly IProviderService providerService;
        ICacheManager<object> cacheManager;
        public ProviderController(IProviderService _providerService,ICacheManager<object> _cache)
        {
            providerService = _providerService;
            cacheManager = _cache;
        }
        #region 供应商管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProvider(Provider model)
        {
            model.UpdatedBy = User.Identity.Name;
            var result = await providerService.AddEntity(model);
            if (result.Success)
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProvider(string id)
        {
            var result = await providerService.DeleteEntityById(id);
            if (result.Success)
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProvider(Provider model)
        {            
            model.UpdatedBy = User.Identity.Name;
            model.ModifiedDate = DateTime.UtcNow;
            var result = await providerService.UpdateEntity(model);
            if (result.Success)
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
        public async Task<JsonResult> GetAll()
        {
            string key = "ProviderList";
            //var data = new List<Object>();
            GetManyResult<Provider> result;
            result = cacheManager.Get(key) as GetManyResult<Provider>;
            if (result == null || !result.Success)
            {
                result = await providerService.ListProviders();
                cacheManager.Add(key, result);
            }
            
            if (result.Success)
            {
                //foreach (var provider in result.Entities)
                //{
                    
                //    var doc = new
                //    {
                //        name  = provider.Name,
                //        value = provider.Id,
                //        disabled = provider.IsArchived
                //    };

                //    data.Add(doc);
                    
                //}
                return DropdownData(result.Success, result.Entities.Select(e => new { name = e.Name + "----" + e.Alias, value = e.Id, disabled = e.IsArchived }).OrderBy(e => e.name));
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
        #endregion       
    }
}