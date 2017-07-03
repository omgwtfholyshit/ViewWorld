using CacheManager.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
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


    }
}
