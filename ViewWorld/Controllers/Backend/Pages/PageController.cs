﻿using System.Threading.Tasks;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Core.Models.ProviderModels;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Models;

namespace ViewWorld.Controllers.Backend.Pages
{
    [Authorize(Roles ="管理员,销售")]
    public class PageController : BaseController
    {
        // GET: Page
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
            var result = await Repo.GetOne<ApplicationUser>(UserId);
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
        public ActionResult SceneryManagement()
        {
            return View();
        }
        #endregion
        public async Task<ActionResult> GlobalSetting()
        {
            var settings = await Repo.GetAll<GlobalSetting>();
            return View(settings.Entities);
        }
        #region 供应商管理
        public async Task<ActionResult> Provider()
        {
            var providers = await Repo.GetAll<Provider>();
            return View(providers.Entities);
        }
        public async Task<ActionResult> DepartureManagement()
        {
            var departures = await Repo.GetAll<StartingPoint>();            
            return View(departures.Entities);
        }
        #endregion        
    }
}