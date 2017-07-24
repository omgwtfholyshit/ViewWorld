using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ViewWorld.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IMongoDbRepository Repo;
        ICacheManager<object> cacheManager;
        public AdminService(IMongoDbRepository _Repo, ICacheManager<object> _cacheManager)
        {
            this.Repo = _Repo;
            this.cacheManager = _cacheManager;
        }
        public Task<GetManyResult<ApplicationUser>> ListUsersByPermissions(params Permission[] permissions)
        {
            var builder = Builders<ApplicationUser>.Filter;
            FilterDefinition<ApplicationUser> filter = builder.Empty;
            foreach(var permission in permissions)
            {
                filter = builder.And(filter, builder.Where(u => u.Permissions.Contains(permission)));
            }
            return Repo.GetManyAsync(filter);
        }

        public Task<GetManyResult<ApplicationUser>> ListUsersByRoles(string roles)
        {
            GetManyResult<ApplicationUser> userList = new GetManyResult<ApplicationUser>();
            if (string.IsNullOrWhiteSpace(roles))
            {
                return Task.FromResult(userList);
            }else
            {
                var builder = Builders<ApplicationUser>.Filter;
                FilterDefinition<ApplicationUser> filter = builder.Empty;
                if (roles.IndexOf(',') > 0)
                {
                    var roleList = roles.Split(',');
                    filter = builder.AnyIn("Roles", roleList);
                }
                else
                {
                    filter = builder.And(filter, builder.Where(u => u.Roles.Contains(roles)));
                }
                return Repo.GetManyAsync(filter);
            }
        }
        
            
        
    }
}
