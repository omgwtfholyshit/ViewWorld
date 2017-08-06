using CacheManager.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.BusinessModels;
using ViewWorld.Core.Models.Identity;
using ViewWorld.Utils;

namespace ViewWorld.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IMongoDbRepository Repo;
        ICacheManager<object> cacheManager;
        public UserService(IMongoDbRepository _Repo, ICacheManager<object> _cacheManager)
        {
            this.Repo = _Repo;
            this.cacheManager = _cacheManager;
        }
        public async Task<object> GetUserInfo(string UserId)
        {
            var userInfo = cacheManager.Get("userinfo", UserId);
            if (userInfo == null)
            {
                var result = await Repo.GetOneAsync<ApplicationUser>(UserId);
                if (result.Success)
                {
                    if (!System.IO.File.Exists(result.Entity.Avatar))
                        result.Entity.Avatar = "/Images/DefaultImages/UnknownSex.jpg";
                    var data = new
                    {
                        Username = result.Entity.UserName,
                        Nickname = result.Entity.NickName,
                        Avatar = result.Entity.Avatar,
                        Points = result.Entity.Points,
                        Role = result.Entity.Roles.FirstOrDefault()
                    };
                    userInfo = data;
                    cacheManager.Add("userinfo", data, UserId);
                }
            }
            return userInfo;
        }

        public async Task<Result> UpdateUserInfo(string Nickname, SexType Sex, DateTime DOB , string UserId)
        {
            var updateDef = Builders<ApplicationUser>.Update.Set("NickName", Nickname).Set("DOB", DOB).Set("Sex", Sex);
            var result = await Repo.UpdateOneAsync(UserId, updateDef);
            if (result.Success)
            {
                cacheManager.Remove("userinfo", UserId);
            }
            return result;
        }

        public async Task<Result> AddToCollection(string userId, string itemId, string itemName, string memo, ProductType type)
        {
            var existed = await CheckIfItemCollected(userId, itemId);
            if (!existed)
            {
                var collection = new Collection()
                {
                    UserId = userId,
                    ItemId = itemId,
                    ItemName = itemName,
                    Memo = memo,
                    Type = type,
                    CollectedAt = DateTime.Now
                };
                return await Repo.AddOneAsync(collection);
            }
            return new Result() { Success = false, Message = "已加入收藏", ErrorCode = 300 };

        }

        public async Task<bool> CheckIfItemCollected(string userId, string itemId)
        {
            var filter = Builders<Collection>.Filter.Where(c => c.UserId == userId && c.ItemId == itemId); 
            var result = await Repo.GetOneAsync<Collection>(filter);
            return result.Success;
        }

        public async Task<Result> RemoveFromCollection(string userId, string itemId)
        {
            var existed = await CheckIfItemCollected(userId, itemId);
            var result = new Result() { Success = false, Message = "", ErrorCode = 300 };
            if (existed)
            {
                var filter = Builders<Collection>.Filter.Where(c => c.UserId == userId && c.ItemId == itemId);
                result = await Repo.DeleteOneAsync(filter);
            }
            return result;
        }
    }
}
