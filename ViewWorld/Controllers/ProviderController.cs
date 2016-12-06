using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver;
using ViewWorld.Core.Models.ProviderModels;

namespace ViewWorld.Controllers
{
    public class ProviderController : BaseController
    {
        #region 供应商管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProvider(Provider model)
        {
            model.UpdatedBy = User.Identity.Name;
            var result = await Repo.AddOne<Provider>(model);
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
            var update = Builders<Provider>.Update.Set("IsArchived", true).CurrentDate("ModifiedDate");
            var result = await Repo.UpdateOne<Provider>(id, update);
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
            var result = await Repo.ReplaceOne<Provider>(model.Id, model);
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
            var result = await Repo.GetAll<Provider>();
            var data = new List<Object>();
            
            if (result.Success)
            {
                foreach (var provider in result.Entities)
                {
                    
                    var doc = new
                    {
                        name  = provider.Name,
                        value = provider.Id,
                        disabled = provider.IsArchived
                    };

                    data.Add(doc);
                    
                }
                return DropdownData(result.Success, data);
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
        #endregion       
    }
}