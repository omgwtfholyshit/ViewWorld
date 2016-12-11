
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.App_Start
{
    public sealed class DataInitializer
    {
        IMongoDbRepository Repo;
        public DataInitializer(IMongoDbRepository _repo)
        {
            Repo = _repo;
        }
       
       public void CreatePermissions()
        {
            List<PermissionStore> permissionList = new List<PermissionStore>();
            permissionList.Add(new PermissionStore { Id = ObjectId.GenerateNewId().ToString(), Permission = new Permission { Name = "FullAccess", ChineseName = "所有权限", Description = "本系统的最高管理权限" } });
            permissionList.Add(new PermissionStore { Id=ObjectId.GenerateNewId().ToString(),Permission = new Permission { Name = "Region", ChineseName = "区域管理", Description = "管理区域分类的权限" } });
            permissionList.Add(new PermissionStore { Id = ObjectId.GenerateNewId().ToString(), Permission = new Permission { Name = "Provider", ChineseName = "供应商管理", Description = "修改供应商的权限" } });
            permissionList.Add(new PermissionStore { Id = ObjectId.GenerateNewId().ToString(), Permission = new Permission { Name = "Trip", ChineseName = "行程管理", Description = "修改行程的权限" } });
            permissionList.Add(new PermissionStore { Id = ObjectId.GenerateNewId().ToString(), Permission = new Permission { Name = "Scenery", ChineseName = "景点管理", Description = "修改景点的权限" } });

            Repo.AddMany(permissionList);
        }
        public static void Init()
        {
           // CreateSceneries();
           // Task.Run(new Action(async () => await new DataInitializer().CreateSceneriesAsync()));
        }
    }
}