using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Admin
{
    public interface IAdminService
    {
        Task<GetManyResult<ApplicationUser>> ListUsersByRoles(string roles);
        Task<GetManyResult<ApplicationUser>> ListUsersByPermissions(params Permission[] permissions);
    }
}
