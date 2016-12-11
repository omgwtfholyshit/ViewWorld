using AspNet.Identity.MongoDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;

namespace ViewWorld.Core.Models.Identity
{
    public class ViewWorldPrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }
        List<Permission> _PermissionList;
        IMongoDbRepository Repo;
        List<string> Roles;
        public ViewWorldPrincipal(string UserId)
        {
            Repo = new MongoDbRepository(new ApplicationIdentityContext()) as IMongoDbRepository;
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                var result = Repo.GetOne<ApplicationUser>(UserId);
                if (result.Entity != null)
                {
                    Roles = result.Entity.Roles;
                    this._PermissionList = result.Entity.Permissions;
                    Identity = new ViewWorldIdentity(result.Entity.UserName, result.Entity.Department, result.Entity.PhoneNumber);
                }
                    
            }else
            {
                Identity = new ViewWorldIdentity("", "", "");
            }
            
        }
        public List<Permission> PermissionList
        {
            get { return _PermissionList; }
            set { _PermissionList = value; }
        }
        public bool IsInRole(string role)
        {
            if (Roles == null || Roles.Count() == 0)
            {
                return false;
            }
            return Roles.Contains(role);
        }
        public bool HasPermission(Permission item)
        {
            return PermissionList.Contains(item);
        }
        public bool HasPermission(string permissionName)
        {
            if (PermissionList == null || PermissionList.Count() == 0)
                return false;
            if (!permissionName.Contains(','))
            {
                return PermissionList.Where(p => p.Name == permissionName).Any();
            }else
            {
                string[] permissions = permissionName.Split(',').ToArray();
                bool hasPermission = true;
                foreach(var permission in permissions)
                {
                    if(!PermissionList.Where(p => p.Name == permission).Any())
                    {
                        hasPermission = false;
                        break;
                    }
                }
                return hasPermission;
            }
            
        }
    }
}
