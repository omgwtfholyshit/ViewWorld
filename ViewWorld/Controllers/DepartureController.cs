using System.Web.Mvc;
using System.Threading.Tasks;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Controllers
{
    public class DepartureController : BaseController
    {
        public async Task<ActionResult> AddDeparture(StartingPoint data)
        {
            //data.UpdatedBy = User.Identity.Name;
            //var result = await Repo.AddOne<StartingPoint>(data);
            //if (result.Success)
            //{
            //    return SuccessJson();
            //}
            //else
            //{
            //    return ErrorJson(result.Message);
            //}
            return SuccessJson();
        }
    }
}