using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Models;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ViewWorld.Controllers
{
    public class ProviderController : BaseController
    {
        #region Constructor
        MongoRepository repo;
        public ProviderController()
            : this(new MongoRepository())
        {

        }
        public ProviderController(MongoRepository _repo)
        {
            repo = _repo;
        }
        #endregion
        #region 供应商管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProvider(Provider model)
        {
            model.UpdatedBy = User.Identity.Name;
            var result = await repo.AddOne<Provider>(model);
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
            var result = await repo.UpdateOne<Provider>(id, update);
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
            var result = await repo.ReplaceOne<Provider>(model.Id, model);
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
            var result = await repo.GetAll<Provider>();
            var data = new List<Object>();
            
            if (result.Success)
            {
                foreach (var provider in result.Entities)
                {
                    if (!provider.IsArchived)
                    {
                        var doc = new
                        {
                            name  = provider.Name,
                            value = provider.Id
                        };

                        data.Add(doc);
                    }
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