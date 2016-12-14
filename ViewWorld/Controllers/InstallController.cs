using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Dal;
using ViewWorld.Services.Installation;

namespace ViewWorld.Controllers
{
    public class InstallController : BaseController
    {
        // GET: Install
        readonly IInstallService installService;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public InstallController(IInstallService _service)
        {
            installService = _service;
        }
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Processing()
        {
            var watch = new Stopwatch();
            watch.Start();
            var userList = installService.GenerateUserList();
            int successCount = 0;
            #region 创建用户
            Response.Write("<p>******添加用户******</p>");
            foreach (var user in userList)
            {
                var result = UserManager.Create(user, "Qqq111qqq");
                if (result.Succeeded)
                    successCount++;
            }
            Response.Write(string.Format("<p>******{0}个用户已添加******</p>", successCount));
            #endregion
            #region 用户权限
            Response.Write("<p>******添加用户权限******</p>");
            var PermissionStoreResult = await installService.InsertPermissionStoreData();
            if (PermissionStoreResult.Success)
            {
                Response.Write("<p>******用户权限添加完成******</p>");
            }
            else
            {
                Response.Write("******用户权限添加失败******\n " + PermissionStoreResult.Message);
            }
            #endregion
            #region 供应商
            Response.Write("<p>******开始添加供应商******</p>");
            var ProviderResult = await installService.InsertProviderData();
            if (ProviderResult.Success)
            {
                Response.Write("<p>******供应商添加完成******</p>");
            }
            else
            {
                Response.Write("******供应商添加失败******\n " + ProviderResult.Message);
            }
            #endregion
            #region 区域数据
            Response.Write("<p>******开始添加区域数据******</p>");
            var RegionResult = await installService.InsertRegionData();
            if (RegionResult.Success)
            {
                Response.Write("<p>******区域添加完成******</p>");
            }
            else
            {
                Response.Write("<p>******区域添加失败******</p> " + RegionResult.Message);
            }
            #endregion
            #region 景点,出发点
            var SceneryResult = await installService.InsertSceneryData();
            var StartingPointResult = await installService.InsertStartingPointData();
            return Content(string.Format("<p>数据安装完成,用时{0}秒</p>",watch.Elapsed.TotalSeconds));
            #endregion
        }
        public async Task<ActionResult> DataBasePerformanceTest()
        {
            Stopwatch timer = new Stopwatch();
            for(int i = 0; i < 9999; i++) {
                await Task.Run(() => installService.InsertProviderData());
            }
            return Content("输入30000条数据用时共{0}秒", timer.Elapsed.TotalSeconds.ToString());
        }
    }
}