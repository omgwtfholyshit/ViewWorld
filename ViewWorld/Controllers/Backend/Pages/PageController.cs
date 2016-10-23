using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models;
using ViewWorld.Models;

namespace ViewWorld.Controllers.Backend.Pages
{
    public class PageController : BaseController
    {
        MongoRepository repo;
        public PageController()
            :this(new MongoRepository())
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
        public async Task<ActionResult> UserProfile()
        {
            var result = await repo.GetOne<ApplicationUser>(UserId);
            return View(result.Entity);
        }
        public ActionResult EditPassword()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult RegionManagement()
        {
            return View();
        }
    }
}