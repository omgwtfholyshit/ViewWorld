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
        public ViewWorldPrincipal(IMongoDbRepository _repo , string UserId)
        {
            Repo = _repo;
            var result = Repo.GetOne<ApplicationUser>(UserId);
            Identity = new ViewWorldIdentity(result.Entity.UserName,result.Entity.Department,result.Entity.PhoneNumber);
        }
        public List<Permission> PermissionList
        {
            get { return _PermissionList; }
        }
        public bool IsInRole(string role)
        {
            return false;
        }
        public bool HasPermission(Permission item)
        {
            return PermissionList.Contains(item);
        }
    }
}
