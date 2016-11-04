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
        public TripController():this(new TripManager())
        {

        }
        #endregion
        #region 区域管理
        public async Task<JsonResult> ListRegions(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(await tripManager.GetRegions());
            }
            return Json(await tripManager.SearchRegions(keyword));
        }
        #endregion

    }
}