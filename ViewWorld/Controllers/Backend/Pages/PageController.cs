﻿using System.Threading.Tasks;
using System.Web.Mvc;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models;
using ViewWorld.Core.Models.Identity;
using ViewWorld.Core.Models.ProviderModels;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Models;
using ViewWorld.Core.ExtensionMethods;
using System.Linq;
using MongoDB.Driver;
using System.Security.Claims;

namespace ViewWorld.Controllers.Backend.Pages
{
    [Authorize(Roles ="管理员,销售")]
    public class PageController : BaseController
    {
        // GET: Page
        private readonly IMongoDbRepository Repo;
        public PageController(IMongoDbRepository _repo)
        {
            Repo = _repo;
        }
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(UserRole.Admin) || User.IsInRole(UserRole.Sales))
                {
                    return RedirectToAction("UserProfile", "Page");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        #region 账户管理
        public async Task<ActionResult> UserProfile()
        {
            var result = await Repo.GetOneAsync<ApplicationUser>(UserId);
            if (!System.IO.File.Exists(result.Entity.Avatar))
                result.Entity.Avatar = "/Images/DefaultImages/UnknownSex.jpg";
            return View(result.Entity);
        }
        public ActionResult EditPassword()
        {
            return View();
        }
        #endregion
        #region 旅游管理
        public ActionResult RegionManagement()
        {
            return View();
        }
        public ActionResult TripManagement()
        {
            return View();
        }
        public ActionResult EditTripManagement(string TripId)
        {
            return View();
        }
        public ActionResult SceneryManagement()
        {
            return View();
        }
        public async Task<ActionResult> CityManagement()
        {
            var cities = await Repo.GetAllAsync<CityInfo>();
            return View(cities.Entities);
        }
        #endregion
        public async Task<ActionResult> GlobalSetting()
        {
            var settings = await Repo.GetAllAsync<GlobalSetting>();
            return View(settings.Entities);
        }
        #region 供应商管理
        public async Task<ActionResult> Provider()
        {
            var providers = await Repo.GetAllAsync<Provider>();
            return View(providers.Entities);
        }
        public async Task<ActionResult> DepartureManagement(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var departures = await Repo.GetAllAsync<StartingPoint>();
                return View(departures.ManyToListResult().Entities.Where(depart => !depart.IsArchived).OrderByDescending(depart => depart.AddedDate));
            }else
            {
                var builder = Builders<StartingPoint>.Filter;
                keyword = keyword.ToUpper();
                FilterDefinition<StartingPoint> filter = builder.Where(point => point.Address.ToUpper().Contains(keyword) || point.ProviderName.ToUpper().Contains(keyword) || point.ProviderAlias.ToUpper().Contains(keyword)||point.Landmark.ToUpper().Contains(keyword));
                var result = (await Repo.GetManyAsync(filter)).ManyToListResult();
                return View(result.Entities.Where(depart => !depart.IsArchived).OrderByDescending(depart => depart.AddedDate));
            }
            
        }
        #endregion
        #region 订单管理
        public ActionResult Orders()
        {
            ViewBag.UserId = UserId;
            ViewBag.Role = GetClaimValue(ClaimTypes.Role);
            return View();
        }
        #endregion
    }
}