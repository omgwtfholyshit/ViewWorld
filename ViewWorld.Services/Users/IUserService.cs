using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Users
{
    public interface IUserService
    {
        Task<Result> UpdateUserInfo(string Nickname, SexType Sex, DateTime DOB, string UserId);

        Task<object> GetUserInfo(string UserId);
    }
}
