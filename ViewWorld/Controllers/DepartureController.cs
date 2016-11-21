using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Models;
using ViewWorld.Models.Trip;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ViewWorld.Controllers
{
    public class DepartureController : BaseController
    {
        MongoRepository repo;
        public DepartureController()
            :this (new MongoRepository())
        {

        }
        public DepartureController(MongoRepository _repo)
        {
            repo = _repo;
        }
        public async Task<ActionResult> AddDeparture(StartingPoint data)
        {
            data.UpdatedBy = User.Identity.Name;
            var result = await repo.AddOne<StartingPoint>(data);
            if (result.Success)
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(result.Message);
            }
        }
    }
}