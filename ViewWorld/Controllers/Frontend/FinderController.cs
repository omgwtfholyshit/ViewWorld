using CacheManager.Core;
using MongoDB.Driver;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Core.Models.ViewModels;
using ViewWorld.Services.Cities;
using ViewWorld.Services.Trips;
using ViewWorld.Utils;

namespace ViewWorld.Controllers.Frontend
{
    public class FinderController : BaseController
    {
        ICacheManager<object> cacheManager;
        ICityService cityService;
        ITripService tripService;
        public FinderController(ICacheManager<object> _cache, ICityService _city,ITripService _trip)
        {
            cacheManager = _cache;
            cityService = _city;
            tripService = _trip;
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
        public async Task<ActionResult> TripDetail(string productId)
        {
            var trip = (await tripService.RetrieveEntitiesByKeyword(productId)).Entities.FirstOrDefault();
            if (trip != null)
            {
                ViewBag.departCity = TripService.GetCity(trip.ProductInfo.DepartingCity);
                ViewBag.arrivalCity = TripService.GetCity(trip.ProductInfo.ArrivingCity);
                if (trip.TripPlans.Any())
                {
                    string tripData = "{";
                    foreach (var plan in trip.TripPlans)
                    {
                        foreach(var item in plan.TripPrices)
                        {
                            tripData += string.Format("%22{0}%22" + ":"+ "%22{1}_{2}%22,", item.TripDate.ToString("MM-dd-yyyy"), trip.CommonInfo.ShortPriceType + item.BasePrice.QuadplexPrice.ToString(),plan.Id);
                        }
                    }
                    ViewBag.tripData = tripData.TrimEnd(',') + "}";
                }
                
                return View(trip);
            }
                
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
        [HttpGet]
        [OutputCache(VaryByParam = "isCncity;initial", Duration = 3600)]
        public async Task<ActionResult> RenderCityPartial(string initial, bool isCnCity = false)
        {
            var data = await cityService.GetCitiesByGroup(initial, isCnCity);
            return PartialView("~/Views/PartialViews/_PartialCitySelection.cshtml", data);
        }

        public async Task<JsonResult> GetTripsBySearchModel(FinderViewModels model,int pageNum = 1)
        {
            var data = await tripService.RetrieveTripArrangementByFilter(model);
            return PageJson(data.Entities.ToPagedList(pageNum, 3));
        }
        public async Task<JsonResult> CalculateTripPrice(List<PeoplePerRoomViewModel> rooms, DateTime departDate, string tripId, string planId)
        {
            try
            {
                var price = await tripService.CalculateTripPrice(rooms, departDate, tripId, planId);
                return Json(price);
            }catch(ArgumentOutOfRangeException ex)
            {
                return ErrorJson(ex.Message);
            }
        }
    }
}