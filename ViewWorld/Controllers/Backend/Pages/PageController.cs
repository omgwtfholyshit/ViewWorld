using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ViewWorld.Controllers.Backend.Pages
{
    [Authorize(Roles ="管理员,销售")]
    public class PageController : BaseController
    {
        MongoRepository repo;
        public PageController()
            : this(new MongoRepository())
        {

        }
        public PageController(MongoRepository _repo)
        {
            repo = _repo;
        }
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
            var result = await repo.GetOne<ApplicationUser>(UserId);
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
            var settings = await repo.GetAll<GlobalSetting>();
            return View(settings.Entities);
        }
        #region 供应商管理
        public async Task<ActionResult> Provider()
        {
            var providers = await repo.GetAll<Provider>();
            return View(providers.Entities);
        }
        public ActionResult DepartureManagement()
        {
            return View();
        }
        #endregion
    }
}