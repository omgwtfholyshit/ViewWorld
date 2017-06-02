using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Utils;
using System.Web;
using System.Web.Script.Serialization;
using ViewWorld.Core.ExtensionMethods;
using MongoDB.Driver;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using ViewWorld.Core.Models.ViewModels;
using CacheManager.Core;

namespace ViewWorld.Services.Trips
{
    public class TripService : ITripService
    {
        private readonly IMongoDbRepository Repo;
        ICacheManager<GetManyResult<TripArrangement>> cacheManager;
        const string dataDirectory = "/Upload/Trips/";
        public TripService(IMongoDbRepository _Repo,ICacheManager<GetManyResult<TripArrangement>> _cacheManager)
        {
            this.Repo = _Repo;
            this.cacheManager = _cacheManager;
        }

        public async Task<Result> AddEntity(TripArrangement Entity)
        {
            var uuid = ObjectId.GenerateNewId();
            Entity.Id = uuid.ToString();
            Entity.ProductId = uuid.Increment.ToString();
            var result = await Repo.AddOneAsync(Entity);
            if (result.Success)
            {
                result.Message = Entity.Id;
            }
            return result;
        }

        public async Task<Result> DeleteEntityById(string id)
        {
            var folder = PathHelper.MapPath(dataDirectory + id);
            if (Directory.Exists(folder))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                try
                {
                    directoryInfo.Delete(true);
                }
                catch (Exception e)
                {
                    Tools.WriteLog("TripArrangement", "删除Trip时清理其图片", e.Message);
                }

            }
            return await Repo.DeleteOneAsync<TripArrangement>(id);
        }

        public async Task<Result> RetrieveTripArrangementById(string tripId)
        {
            var result = new Result() { ErrorCode = 300, Message = "", Success = false };
            if (string.IsNullOrWhiteSpace(tripId))
            {
                result.Message = "Id不能为空";
                return result;
            }
            return await Repo.GetOneAsync<TripArrangement>(tripId);
        }

        public async Task<GetListResult<TripArrangement>> RetrieveEntitiesByKeyword(string keyword)
        {
            GetManyResult<TripArrangement> result = cacheManager.Get("Trips", "Front");
            GetListResult<TripArrangement> listResult = new GetListResult<TripArrangement>() {
                Success = false, ErrorCode = 300, Entities = new List<TripArrangement>(), Message = "" };
            if (result == null || !result.Success)
            {
                result = await Repo.GetAllAsync<TripArrangement>();
            }
            if (result.Success)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return result.ManyToListResult();
                else
                {
                    keyword = keyword.ToUpper();
                    int productId = 0;
                    if (int.TryParse(keyword, out productId))
                    {
                        var trip = result.Entities.SingleOrDefault(t => t.ProductId == keyword);
                        if (trip != null)
                        {
                            listResult.Entities.Add(trip);
                            listResult.Success = true;
                            listResult.ErrorCode = 200;
                        }
                       
                    }
                    else
                    {
                        var trips = result.Entities.Where(obj => obj.CommonInfo.Name.Contains(keyword) || obj.Publisher.Contains(keyword));
                        if (trips != null)
                        {
                            listResult.Entities.AddRange(trips);
                            listResult.Success = true;
                            listResult.ErrorCode = 200;
                        }
                    }

                }
            }
            return listResult;
        }

        /// <summary>
        /// 根据SearchModel遍历并筛选缓存中的list。数据多了之后测试RetrieveTripArrangementBySearchModel的效率
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TripArrangement>> RetrieveTripArrangementBySearchModel(FinderViewModels model)
        {
           
            IEnumerable<TripArrangement> filteredTrips = new List<TripArrangement>();
            GetManyResult<TripArrangement> result = await GetCachedResult();
            if (result.Success)
            {
                if (result.Entities == null || result.Entities.Count() <= 0)
                {
                    result.Message = "No trips located.";

                }
                else
                {
                    filteredTrips = result.Entities.Where(t => t.ProductInfo.DepartingCity.Contains(model.DepartureCity));
                    if (!string.IsNullOrWhiteSpace(model.Region))
                        filteredTrips = filteredTrips.Where(t => model.Region.Contains(t.CommonInfo.RegionId));
                    if (model.Days > 0)
                        filteredTrips = filteredTrips.Where(t => t.ProductInfo.TotalDays >= model.Days);
                    if (!string.IsNullOrWhiteSpace(model.ArrivalCity))
                        filteredTrips = filteredTrips.Where(t => t.ProductInfo.ArrivingCity.Contains(model.ArrivalCity));
                    if (!string.IsNullOrWhiteSpace(model.keyword))
                        filteredTrips = filteredTrips.Where(t => t.CommonInfo.Name.Contains(model.keyword) || t.CommonInfo.Keyword.Contains(model.keyword));
                    if (!string.IsNullOrWhiteSpace(model.Theme))
                        filteredTrips = filteredTrips.Where(t => t.ProductInfo.Sceneries.Contains(model.Theme));
                }
            }
            return filteredTrips;

        }
        [Obsolete("Search via Filter, Please compare the efficiency with RetrieveTripArrangementBySearchModel when trip number grows up", false)]
        public async Task<GetManyResult<TripArrangement>> RetrieveTripArrangementByFilter(FinderViewModels model)
        {
            var builder = Builders<TripArrangement>.Filter;
            FilterDefinition<TripArrangement> filter = builder.Where(t => !t.IsDeleted && !t.IsVisible);
            if(!string.IsNullOrWhiteSpace(model.DepartureCity))
                filter = builder.And(filter, builder.Where(t => t.ProductInfo.DepartingCity.Contains(model.DepartureCity)));
            if (!string.IsNullOrWhiteSpace(model.Region))
                filter = builder.And(filter, builder.Where(t => t.CommonInfo.RegionName == model.Region));
            if (model.Days > 0)
                filter = builder.And(filter, builder.Where(t => t.ProductInfo.TotalDays >= model.Days));
            if (!string.IsNullOrWhiteSpace(model.ArrivalCity))
                filter = builder.And(filter, builder.Where(t => t.ProductInfo.ArrivingCity.Contains(model.ArrivalCity)));
            if (!string.IsNullOrWhiteSpace(model.keyword))
                filter = builder.And(filter, builder.Where(t => t.CommonInfo.Name.Contains(model.keyword) || t.CommonInfo.Keyword.Contains(model.keyword)));
            if (!string.IsNullOrWhiteSpace(model.Theme))
                filter = builder.And(filter, builder.Where(t => t.CommonInfo.Theme.Contains(model.Theme)));
            return await Repo.GetManyAsync(filter);
        }
        public async Task<Result> UpdateEntity(TripArrangement Entity)
        {
            cacheManager.Update("Trips", "Front", result => UpdateCachedResult(result, Entity));
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }

        public async Task<Result> UpdateTripPartial(string tripId, CommonInfo data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.CommonInfo = data;
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "通用信息";
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                return updateResult;
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, ProductInfo data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.ProductInfo = data;
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "产品概要";
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                return updateResult;
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, List<Schedule> data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.Schedules = data;
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "单日行程";
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                return updateResult;
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, List<TripPlan> data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.TripPlans = data;
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "发团计划";
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                return updateResult;
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, TripProperty data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.TripProperty = data;
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "发团属性";
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                return updateResult;
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }
        public async Task<Result> CopyTripArrangement(string tripId)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string tmp = js.Serialize(result.Entity);

                TripArrangement obj = js.Deserialize<TripArrangement>(tmp);
                var uuid = ObjectId.GenerateNewId();
                obj.Id = uuid.ToString();
                obj.ProductId = uuid.Increment.ToString();
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "NickName")
                        obj.Publisher = claim.Value.ToUpper();
                }
                obj.PublisherId = HttpContext.Current.User.Identity.GetUserId();
                obj.PublishedAt = DateTime.Now;
                if (obj.CommonInfo.Photos.Count() > 0)
                {
                    string savePath = PathHelper.MapPath(dataDirectory + obj.Id + "/TripPhotos/");
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    foreach (var photo in obj.CommonInfo.Photos)
                    {
                        string filePath = PathHelper.MapPath(photo.FileLocation);
                        string newPath = savePath + Tools.GenerateId_M1() + Path.GetExtension(photo.Name).ToLower();
                        if (File.Exists(filePath))
                        {
                            File.Copy(filePath, newPath);
                            photo.FileLocation = PathHelper.absolutePathtoVirtualPath(newPath);
                        }else
                        {
                            obj.CommonInfo.Photos.Remove(photo);
                        }
                    }
                }
                await Repo.AddOneAsync(obj);
                result.Message = obj.Id;
            }
            return result;
        }
        public async Task<Result> UploadPhoto(HttpRequestBase request)
        {
            var result = new Result() { ErrorCode = 300, Message = "", Success = false };
            var tripId = request["tripId"];
            var photoJson = request["photoInfo"];
            var serializer = new JavaScriptSerializer();
            var photoInfo = serializer.Deserialize<CommonInfo.PhotoInfo>(photoJson);
            var tripResult = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (tripResult.Success)
            {
                HttpPostedFileBase file;
                try
                {
                    for (int i = 0; i < request.Files.AllKeys.Count(); i++)
                    {
                        file = request.Files[i];
                        string savePath = PathHelper.MapPath(dataDirectory + tripId + "/TripPhotos/");
                        //检测文件是否合法
                        if (!ImageHelper.CheckImageByFileExtension(Path.GetExtension(file.FileName)))
                        {
                            //如果不合法
                            result.Message += file.FileName + ',';
                        }
                        else
                        {
                            string filePath = savePath + Tools.GenerateId_M1() + Path.GetExtension(file.FileName).ToLower();
                            if (!Directory.Exists(savePath))
                                Directory.CreateDirectory(savePath);
                            file.SaveAs(filePath);
                            photoInfo.FileLocation = PathHelper.absolutePathtoVirtualPath(filePath);
                            tripResult.Entity.CommonInfo.Photos.Add(photoInfo);
                            await UpdateEntity(tripResult.Entity);
                            cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, tripResult.Entity));
                        }
                    }
                    if (string.IsNullOrWhiteSpace(result.Message))
                    {
                        result.Success = true;
                        result.ErrorCode = 200;
                        result.Message = photoInfo.FileLocation;
                    }
                    return result;
                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                    return result;
                }
            }
            else
            {
                result.Message = "找不到对应行程";
                return result;
            }
            
        }

        public async Task<Result> DeletePhotoById(string tripId,string photoId)
        {
            var outcome = new Result() { ErrorCode = 300, Message = "", Success = false };
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                var photoInfo = result.Entity.CommonInfo.Photos.FirstOrDefault(p => p.Id == photoId);
                if (photoInfo != null)
                {
                    string photoPath = PathHelper.MapPath(photoInfo.FileLocation);
                    if (File.Exists(photoPath))
                    {
                        File.Delete(photoPath);
                    }
                    result.Entity.CommonInfo.Photos.Remove(photoInfo);
                    await UpdateEntity(result.Entity);
                    cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                    outcome.Success = true;
                    outcome.ErrorCode = 200;
                }
                outcome.Message = "找不到对应图片";
            }
            else
            {
                outcome.Message = "找不到对应行程";
            }
            return outcome;
        }

        public async Task<Result> ToggleTripArrangement(string tripId)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.IsVisible = !result.Entity.IsVisible;
                await UpdateEntity(result.Entity);
                cacheManager.Update("Trips", "Front", r => UpdateCachedResult(r, result.Entity));
                if (result.Entity.IsVisible)
                {
                    result.Message = "隐藏线路";
                }else
                {
                    result.Message = "发布线路";
                }
            }
            return result;
        }
        async Task<GetManyResult<TripArrangement>> GetCachedResult()
        {
            GetManyResult<TripArrangement> result = cacheManager.Get("Trips", "Front");
            if (result == null || !result.Success)
            {
                result = await Repo.GetAllAsync<TripArrangement>();
                cacheManager.Add("Trips", result, "Front");
            }
            return result;
        }
        GetManyResult<TripArrangement> UpdateCachedResult(GetManyResult<TripArrangement> cachedResult,TripArrangement entityToUpdate)
        {
            var update = cachedResult.Entities.FirstOrDefault(e => e.Id == entityToUpdate.Id);
            if (update != null)
                update = entityToUpdate;
            return cachedResult;
        }

        public static string GetCity(string cityStr)
        {
            var cityName = "";
            if (!string.IsNullOrWhiteSpace(cityStr))
            {
                var cityArray = cityStr.Split('|');
                foreach (var city in cityArray)
                {
                    cityName += city.Split(',')[1] + ",";
                }
                cityName = cityName.TrimEnd(',');
            }
            return cityName;
        }

        public async Task<string> CalculateTripPrice(List<PeoplePerRoomViewModel> rooms, DateTime departDate, string tripId, string planId)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            double finalPrice = 0;
            string finalResult = "";
            if (result.Success)
            {
                finalResult += result.Entity.CommonInfo.PriceType;
                var plan = result.Entity.TripPlans.FirstOrDefault(p => p.Id == planId);
                if (plan != null)
                {
                    var tripPrice = plan.TripPrices.FirstOrDefault(p => p.TripDate == departDate).BasePrice;
                    if (tripPrice != null)
                    {
                        foreach(var room in rooms)
                        {
                            switch(room.Adults + room.Children)
                            {
                                case 1:
                                    finalPrice += tripPrice.SinglePrice;
                                    break;
                                case 2:
                                    finalPrice += tripPrice.DoublePrice;
                                    break;
                                case 3:
                                    finalPrice += tripPrice.TriplePrice;
                                    break;
                                case 4:
                                    finalPrice += tripPrice.QuadplexPrice;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("PeoplePerRoomViewModel", "A maximum of 4 people can stay in one room");
                            }
                            finalPrice -= room.Children * tripPrice.ChildPrice;
                            finalPrice += tripPrice.RoomDifference;
                        }
                    }
                }
            }
            return finalResult + finalPrice.ToString();
        }
    }
}
