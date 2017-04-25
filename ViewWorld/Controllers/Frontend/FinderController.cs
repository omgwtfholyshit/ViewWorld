using CacheManager.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Services.Cities;
using ViewWorld.Utils;

namespace ViewWorld.Controllers.Frontend
{
    public class FinderController : BaseController
    {
        ICacheManager<object> cacheManager;
        ICityService cityService;
        public FinderController(ICacheManager<object> _cache, ICityService _city)
        {
            cacheManager = _cache;
            cityService = _city;
        }
        // GET: Finder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FindTrips()
        {
            ViewBag.loc = "北京";
            var ipCookie = Request.Cookies.Get("UserIP");
            if (ipCookie == null || string.IsNullOrWhiteSpace(ipCookie.Value))
            {
                Response.Cookies.Set(new HttpCookie("UserIP", Request.UserHostAddress));
            }
            else
            {
                if (ipCookie.Value == Request.UserHostAddress)
                {
                    var locCookie = Request.Cookies.Get("UserLoc");
                    if (locCookie != null && !string.IsNullOrWhiteSpace(locCookie.Value))
                    {
                        ViewBag.loc = locCookie.Value;
                    }else
                    {
                        ViewBag.loc = HttpHelper.RequestUserLocation(Request.UserHostAddress).City;
                    }
                }
                else
                {
                    ipCookie.Value = Request.UserHostAddress;
                    Response.Cookies.Set(ipCookie);
                    ViewBag.loc = HttpHelper.RequestUserLocation(Request.UserHostAddress).City;
                    Response.SetCookie(new HttpCookie("UserLoc", ViewBag.loc));
                }
            }
            return View();
        }
        [HttpGet]
        [OutputCache(VaryByParam = "isCncity;initial", Duration = 3600)]
        public async Task<ActionResult> RenderCityPartial(string initial, bool isCnCity = false)
        {
            var data = await cityService.GetCitiesByGroup(initial, isCnCity);
            return PartialView("~/Views/PartialViews/_PartialCitySelection.cshtml", data);
        }
    }
}