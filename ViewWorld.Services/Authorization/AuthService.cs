using MongoDB.Driver;
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

        public async Task<Result> AddOnePermissionToUserAsync(string userId, string permissionName)
        {
            FilterDefinition<PermissionStore> fil = Builders<PermissionStore>.Filter.Eq("Permission.Name", permissionName);
            var permission = (await Repo.GetOneAsync<PermissionStore>(fil)).Entity.Permission;
            return await AddOnePermissionToUserAsync(userId, permission);
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
            var result = await Repo.GetOneAsync<ApplicationUser>(userId);
            var outcome = new Result() { ErrorCode = 300, Success = false, Message = "先选择需要删除的权限" };
            if (permissionList!=null && permissionList.Count() > 0)
            {
                foreach (var permission in permissionList)
                {
                    if (result.Entity.Permissions.Contains(permission))
                        result.Entity.Permissions.Remove(permission);
                }
                outcome = await Repo.ReplaceOneAsync<ApplicationUser>(userId, result.Entity);
                outcome.Message = "选中权限已删除";
            }

            return outcome;
        }

        public async Task<Result> DeleteOnePermissionAsync(string Id)
        {
            return await Repo.DeleteOneAsync<PermissionStore>(Id);
        }
        public async Task<Result> DeleteOnePermissionFromUserAsync(string userId, string permissionName)
        {
            try
            {
                var result = await Repo.GetOneAsync<ApplicationUser>(userId);
                Permission permission = result.Entity.Permissions.Where(p => p.Name.ToLower() == permissionName.ToLower()).SingleOrDefault();
                if (permission != null)
                {
                    result.Entity.Permissions.Remove(permission);
                    return await Repo.ReplaceOneAsync<ApplicationUser>(userId, result.Entity);
                }
                    
                return new Result() { ErrorCode = 300, Success = false, Message = "找不到该权限" };
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }
        public async Task<Result> DeleteOnePermissionFromUserAsync(string userId, Permission permission)
        {
            var result = await Repo.GetOneAsync<ApplicationUser>(userId);
            if (result.Entity.Permissions.Contains(permission))
                result.Entity.Permissions.Remove(permission);
            return await Repo.ReplaceOneAsync<ApplicationUser>(userId, result.Entity);
        }

        public async Task<Result> UpdateOnePermissionAsync(string Id, Permission permission)
        {
            var result = await Repo.GetOneAsync<PermissionStore>(Id);
            result.Entity.Permission = permission;
            return await Repo.ReplaceOneAsync(Id, result.Entity);
        }
       
    }
}
