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

        public async Task<GetListResult<TripArrangement>> RetrieveEntitiesByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return (await Repo.GetAllAsync<TripArrangement>()).ManyToListResult();
            }else
            {
                var builder = Builders<TripArrangement>.Filter;
                FilterDefinition<TripArrangement> filter;
                keyword = keyword.ToUpper();
                int productId = 0;
                if(int.TryParse(keyword,out productId))
                {
                    filter = builder.Eq("ProductId", keyword);
                }else
                {
                    filter = builder.Where(obj => obj.CommonInfo.Name.Contains(keyword) || obj.Publisher.Contains(keyword));
                }
                return (await Repo.GetManyAsync(filter)).ManyToListResult();
            }
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
                var updateResult = await UpdateEntity(result.Entity);
                updateResult.Message = "通用信息";
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

        public async Task<Result> ToggleTripArrangement(string tripId)
        {
            var result = await Repo.GetOneAsync<TripArrangement>(tripId);
            if (result.Success)
            {
                result.Entity.IsVisible = !result.Entity.IsVisible;
                await UpdateEntity(result.Entity);
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

        
    }
}
