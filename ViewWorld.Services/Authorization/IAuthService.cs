using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Authorization
{
    public interface IAuthService
    {
        Task<Result> AddOnePermissionAsync(PermissionStore permissionStore);
        Task<Result> UpdateOnePermissionAsync(string Id, Permission permission);
        Task<Result> DeleteOnePermissionAsync(string Id);
        Task<Result> AddOnePermissionToUserAsync(string userId, Permission permission);
        Task<Result> AddManyPermissionsToUserAsync(string userId, List<Permission> permissionList);

        Task<Result> DeleteOnePermissionFromUserAsync(string userId, Permission permission);
        Task<Result> DeleteManyPermissionsFromUserAsync(string userId, List<Permission> permissionList);
    }
}
