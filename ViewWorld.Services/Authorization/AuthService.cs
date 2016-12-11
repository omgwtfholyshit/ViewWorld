using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Authorization
{
    public class AuthService : IAuthService
    {
        readonly IMongoDbRepository Repo;
        public AuthService(IMongoDbRepository _repo)
        {
            this.Repo = _repo;
        }
        public async Task<Result> AddManyPermissionsToUserAsync(string userId, List<Permission> permissionList)
        {
            var result = await Repo.GetOneAsync<ApplicationUser>(userId);
            var outcome = new Result() { ErrorCode = 300, Success = false, Message = "所有权限已存在" };
            if (permissionList != null && permissionList.Count() > 0)
            {
                foreach (var permission in permissionList)
                {
                    if (result.Entity.Permissions.Contains(permission))
                        permissionList.Remove(permission);
                }
                switch (permissionList.Count())
                {
                    case 0:
                        break;
                    case 1:
                        outcome = await AddOnePermissionToUserAsync(userId, permissionList[0]);
                        break;
                    default:
                        outcome = await Repo.AddManyAsync(permissionList);
                        outcome.Message = string.Format("{0} 项权限已添加", permissionList.Count());
                        break;

                }
                return outcome;
            }
            outcome.Message = "没有需要添加的权限";
            return outcome;
        }

        public async Task<Result> AddOnePermissionAsync(PermissionStore permissionStore)
        {
            var result = await Repo.GetAllAsync<PermissionStore>();
            if (!result.Entities.Contains(permissionStore))
            {
                var outcome = await Repo.AddOneAsync(permissionStore);
                outcome.Message = string.Format("{0} 权限已添加", permissionStore.Permission.ChineseName);
                return outcome;
            }
            return new Result() { ErrorCode = 300, Success = false, Message = "该权限已存在" };

        }

        public async Task<Result> AddOnePermissionToUserAsync(string userId, Permission permission)
        {
            var result = await Repo.GetOneAsync<ApplicationUser>(userId);
            if (!result.Entity.Permissions.Contains(permission))
            {
                result.Entity.Permissions.Add(permission);
                return await Repo.ReplaceOneAsync(userId, result.Entity);
            }
            return new Result() { ErrorCode = 300, Success = false, Message = "该权限已存在" };
        }

        public async Task<Result> DeleteManyPermissionsFromUserAsync(string userId, List<Permission> permissionList)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteOnePermissionAsync(string Id)
        {
            return await Repo.DeleteOneAsync<PermissionStore>(Id);
        }

        public async Task<Result> DeleteOnePermissionFromUserAsync(string userId, Permission permission)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateOnePermissionAsync(string Id, Permission permission)
        {
            throw new NotImplementedException();
        }
       
    }
}
