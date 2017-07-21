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
using ViewWorld.Services.Sceneries;
using ViewWorld.Services.Trips;
using ViewWorld.Utils;
using ViewWorld.Utils.ViewModels;

namespace ViewWorld.Controllers.Frontend
{
    public class FinderController : BaseController
    {
        ICacheManager<object> cacheManager;
        ICityService cityService;
        ITripService tripService;
        ISceneryService sceneryService;
        public FinderController(ICacheManager<object> _cache, ICityService _city, ITripService _trip, ISceneryService _scenery)
        {
            cacheManager = _cache;
            cityService = _city;
            tripService = _trip;
            sceneryService = _scenery;
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
                var vm = new FinderTripDetailViewModel();
                vm.Trip = trip;
                vm.DepartCity = TripService.GetCity(trip.ProductInfo.DepartingCity);
                vm.ArrivalCity = TripService.GetCity(trip.ProductInfo.ArrivingCity);
                var firstAvaiableDate = DateTime.Now;
                if (trip.TripPlans.Any())
                {
                    string tripData = "{";
                    foreach (var plan in trip.TripPlans)
                    {
                        foreach(var item in plan.TripPrices)
                        {
                            if (item.TripDate.ToLocalTime() > DateTime.Now)
                            {
                                if(firstAvaiableDate == DateTime.Now)
                                {
                                    firstAvaiableDate = item.TripDate;
                                }else
                                {
                                    firstAvaiableDate = firstAvaiableDate > item.TripDate ? 
                                        item.TripDate : firstAvaiableDate;
                                }
                                
                                tripData += string.Format("%22{0}%22" + ":" + "%22{1}_{2}%22,", item.TripDate.ToLocalTime().ToString("MM-dd-yyyy"), trip.CommonInfo.ShortPriceType + item.BasePrice.QuadplexPrice.ToString(), plan.Id);
                            }
                        }
                    }
                    vm.FirstAvaiableDate = firstAvaiableDate.ToLocalTime().ToString("MM-dd-yyyy");
                    vm.TripData = tripData.TrimEnd(',') + "}";
                }
                
                return View(vm);
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
            var data = await tripService.RetrieveTripArrangementBySearchModel(model);
            return PageJson(data.OrderByDescending(m=>m.ProductInfo.DepartingCity).ToPagedList(pageNum, 3));
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

        [HttpGet]
        public async Task<JsonResult> GetSceneryDetail(string sceneryId)
        {
            var result = await sceneryService.RetrieveEntitiesById(sceneryId);
            if (result.Success)
            {
                var data = new
                {
                    Name = result.Entity.Name,
                    Address = result.Entity.Address,
                    Coordinate = result.Entity.Coordinate,
                    Description = result.Entity.Description,
                    EnglishName = result.Entity.EnglishName,
                    Photos = result.Entity.Photos
                };
                return Json(data);
            }
            return ErrorJson(result.Message);
        }
    }
}