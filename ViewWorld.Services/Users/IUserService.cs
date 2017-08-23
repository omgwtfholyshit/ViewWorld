using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.BusinessModels;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Users
{
    public interface IUserService
    {
        Task<Result> UpdateUserInfo(string Nickname, SexType Sex, DateTime DOB, string UserId);

        Task<object> GetUserInfo(string UserId);

        Task<Result> AddToCollection(string userId,string itemId, string itemName,string memo, string image, ProductType type);
        Task<bool> CheckIfItemCollected(string userId, string itemId);
        Task<Result> RemoveFromCollection(string userId, string id);
        Task<GetManyResult<Collection>> GetCollection(string userId);
    }
}
