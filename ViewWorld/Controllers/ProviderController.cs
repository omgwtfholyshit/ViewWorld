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
        public async Task<ActionResult> EditProvider(Provider model)
        {
            //var update = Builders<Provider>.Update
            //                .Set("Name", model.Name)
            //                .Set("Alias", model.Alias)
            //                .Set("ContactName", model.ContactName)
            //                .Set("Phone", model.Phone)
            //                .Set("Email", model.Email)
            //                .Set("CommissionRate", model.CommissionRate)
            //                .Set("AwardRatio", model.AwardRatio)
            //                .Set("Description", model.Description)
            //                .Set("UpdatedBy", User.Identity.Name)
            //                .CurrentDate("ModifiedDate");
            //var result = await repo.UpdateOne<Provider>(model.Id, update);
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
        #endregion       
    }
}