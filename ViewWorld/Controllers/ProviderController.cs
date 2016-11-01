using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Models;
using System.Threading.Tasks;

namespace ViewWorld.Controllers
{
    public class ProviderController : BaseController
    {
        MongoRepository repo;
        public ProviderController()
            :this(new MongoRepository())
        {

        }
        public ProviderController(MongoRepository _repo)
        {
            repo = _repo;            
        }
        #region 供应商管理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProvider(Provider model)
        {
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
            var result = await repo.DeleteOne<Provider>(id);
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