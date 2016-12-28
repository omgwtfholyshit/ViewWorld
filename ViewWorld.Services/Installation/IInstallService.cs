using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Services.Installation
{
    public interface IInstallService
    {
        Task<Result> InsertPermissionStoreData();
        List<ApplicationUser> GenerateUserList();
        Task<Result> InsertRegionData();
        Task<Result> InsertSceneryData();
        Task<Result> InsertProviderData();
        Task<Result> InsertStartingPointData();
    }
}
