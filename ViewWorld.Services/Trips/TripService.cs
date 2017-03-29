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

namespace ViewWorld.Services.Trips
{
    public class TripService : ITripService
    {
        private readonly IMongoDbRepository Repo;
        const string dataDirectory = "/Upload/Trips/";
        public TripService(IMongoDbRepository _Repo)
        {
            this.Repo = _Repo;
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

        public Task<GetListResult<TripArrangement>> RetrieveEntitiesByKeyword(string keyword)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> TestMethod()
        {
            var result = new Result() { ErrorCode = 300,Message = "gg",Success=false};
            try
            {
                var tripId = ObjectId.GenerateNewId().ToString();
                #region CommonInfo
                var cInfo = new CommonInfo();
                var photoList = new List<CommonInfo.PhotoInfo>();
                photoList.Add(new CommonInfo.PhotoInfo() { Name = "photo1.jpg", Description = "Heaven Lake" });
                photoList.Add(new CommonInfo.PhotoInfo() { Name = "photo2.jpg", Description = "Hell Lake" });
                List<string> spActivityList = new List<string>();
                spActivityList.Add("跳水|$123");
                spActivityList.Add("跳楼|￥10");
                cInfo.AvailableDates = "1,2,3,4";
                cInfo.CurrencyType = Core.Enum.CurrencyType.美元;
                cInfo.GroupId = "10010";
                cInfo.Include = "Nothing Included";
                cInfo.Exclude = "Everything excluded";
                cInfo.Introduction = "Dont ask stupid question";
                cInfo.Keyword = "test";
                cInfo.LowestPrice = 666;
                cInfo.Photos = photoList;
                cInfo.Points = 100;
                cInfo.Promotion = "a,f,g";
                cInfo.ProviderName = "HelloKitty";
                cInfo.RegionId = "5822f33660c3d82f10fb062d";
                cInfo.RegionName = "青岛";
                cInfo.SelfPayActivities = spActivityList;
                cInfo.Theme = "a,c,e";
                #endregion
                #region ProductInfo
                string[] sids = new string[] { "58ccb76060c3d80b8ca911b0", "58ccb76060c3d80b8ca911a6", "58ccb76060c3d80b8ca9119e" };
                var sResult = await Repo.GetManyAsync<Scenery>(sids);
                var sceneryEntities = sResult.Entities.ToList();
                ProductInfo pInfo = new ProductInfo();
                pInfo.ArrivingCity = "悉尼";
                pInfo.DepartingCity = "青岛";
                pInfo.Feature = "嗨翻全场";
                pInfo.Intro = "从早到晚飞一天";
                pInfo.Sceneries = "";
                pInfo.TotalDays = 2;
                #endregion
                #region ScheduleList
                List<Schedule> scheduleList = new List<Schedule>();
                List<ScheduleItem> siList1 = new List<ScheduleItem>();
                List<ScheduleItem> siList2 = new List<ScheduleItem>();
                var sceneries = new List<Scenery>();
                sceneries.Add(sceneryEntities[0]);
                siList1.Add(new ScheduleItem() { ActivityTime = "9am-9pm", Arrangement = "nothin", Memo = "Bring the money", Sceneries = "" });
                sceneries.RemoveAt(0);
                sceneries.Add(sceneryEntities[1]);
                sceneries.Add(sceneryEntities[2]);
                siList2.Add(new ScheduleItem() { ActivityTime = "9am-9pm", Arrangement = "nothin", Memo = "Bring the money", Sceneries = "" });
                scheduleList.Add(new Schedule() { Accommodation = "自理", Day = 1, Description = "吃饭喝酒打屁屁", Details = siList1, GroupPickUp = "没有", Introduction = "烦死了", Meal = "不管", PickUp = "屁话真多", Id = ObjectId.GenerateNewId().ToString() });
                scheduleList.Add(new Schedule() { Accommodation = "自理2", Day = 2, Description = "吃饭喝酒打屁屁2", Details = siList2, GroupPickUp = "没有2", Introduction = "烦死了2", Meal = "不管2", PickUp = "屁话真多2", Id = ObjectId.GenerateNewId().ToString() });

                #endregion
                #region TripProperty
                var psid = new string[] { "58ccb76060c3d80b8ca9119a", "58ccb76060c3d80b8ca91199", "58ccb76060c3d80b8ca91198" };
                var psid2 = new string[] { "58ccb76060c3d80b8ca91194", "58ccb76060c3d80b8ca91195", "58ccb76060c3d80b8ca91196" };
                var psResult = await Repo.GetManyAsync<Scenery>(psid);
                var psResult2 = await Repo.GetManyAsync<Scenery>(psid);
                var psEntities = psResult.Entities.ToList();
                var psEntities2 = psResult2.Entities.ToList();
                List<TripProperty.AirportPickUp> PickUpInfos = new List<TripProperty.AirportPickUp>();
                PickUpInfos.Add(new TripProperty.AirportPickUp() { PickUpStartAt = DateTime.Now, PickUpEndAt = DateTime.Now.AddHours(3), IsFree = true, Price = 0, Title = "悉尼接机" });
                PickUpInfos.Add(new TripProperty.AirportPickUp() { PickUpStartAt = DateTime.Now, PickUpEndAt = DateTime.Now.AddHours(6), IsFree = false, Price = 50, Title = "悉尼又接机" });
                TripProperty tproperty = new TripProperty();
                tproperty.PickUpInfos = PickUpInfos;
                tproperty.SelectableRoutes.Add("Route1", psEntities);
                tproperty.SelectableRoutes.Add("Route2", psEntities);
                #endregion
                #region TripPlan
                TripPlan.TripPriceForSpecificDate day1 = new TripPlan.TripPriceForSpecificDate()
                {
                    BasePrice = new HotelPrice() { Name = "a hotel", SinglePrice = 100, DoublePrice = 90, TriplePrice = 80, QuadplexPrice = 75, ChildPrice = 0, RoomDifference = 0 },
                    RaisePriceByPercentage = 40,
                    TripDate = DateTime.Now.AddMonths(1),
                    AdditionalPrice = new HotelPrice() { Name = "a hotel", SinglePrice = 0, DoublePrice = 0, TriplePrice = 0, QuadplexPrice = 0, ChildPrice = 0, RoomDifference = 0 }
                };
                TripPlan.TripPriceForSpecificDate day2 = new TripPlan.TripPriceForSpecificDate()
                {
                    BasePrice = new HotelPrice() { Name = "b hotel", SinglePrice = 100, DoublePrice = 90, TriplePrice = 80, QuadplexPrice = 75, ChildPrice = 0, RoomDifference = 0 },
                    RaisePriceByPercentage = 40,
                    TripDate = DateTime.Now.AddMonths(2),
                    AdditionalPrice = new HotelPrice() { Name = "b hotel", SinglePrice = 100, DoublePrice = 90, TriplePrice = 80, QuadplexPrice = 75, ChildPrice = 0, RoomDifference = 10 }
                };
                TripPlan tplan = new TripPlan();
                tplan.CurrencyType = Core.Enum.CurrencyType.美元;
                tplan.Id = ObjectId.GenerateNewId().ToString();
                tplan.OneDayOnly = true;
                tplan.TripId = tripId;
                tplan.TripPrice.Add(day1);
                tplan.TripPrice.Add(day2);
                tplan.Type = Core.Enum.TripTypes.PlanType.指定日期发团;
                #endregion
                var tripModel = new TripArrangement(cInfo, pInfo, scheduleList, tplan, tproperty);
                tripModel.Id = tripId;
                tripModel.ProductId = Tools.GenerateId_M2();
                result = await Repo.AddOneAsync<TripArrangement>(tripModel);
            }catch(Exception e)
            {
                result.Message = e.Message;
            }
            
            return result;
        }

        public async Task<Result> UpdateEntity(TripArrangement Entity)
        {
            return await Repo.ReplaceOneAsync(Entity.Id, Entity);
        }

        public async Task<Result> UpdateTripPartial(string tripId, CommonInfo data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.CommonInfo = data;
                return await UpdateEntity(result.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, ProductInfo data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.ProductInfo = data;
                return await UpdateEntity(result.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, List<Schedule> data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.Schedules = data;
                return await UpdateEntity(result.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, TripPlan data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.TripPlan = data;
                return await UpdateEntity(result.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
        }

        public async Task<Result> UpdateTripPartial(string tripId, TripProperty data)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.TripProperty = data;
                return await UpdateEntity(result.Entity);
            }
            return new Result() { ErrorCode = 300, Message = "找不到该行程", Success = false };
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
    }
}
