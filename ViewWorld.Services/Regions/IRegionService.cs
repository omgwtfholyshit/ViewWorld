using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Regions
{
    public interface IRegionService : ICRUDable<Region>
    {
        Task<Result> DeleteRegion(string id, string parentId);
        Task<Result> UpdateRegion(string id, Region model);
        Task<Result> UpdateSubRegion(Region model);
        Task<Result> ChangeRegion(string parentId, string id, string destId);
        Task<GetManyResult<Region>> GetRegions(bool VisibileOnly = true, bool MainRegionOnly = false);
        Task<GetListResult<Region>> RetrieveEntitiesByKeyword(string keyword,GetManyResult<Region> cacheResult);
    }
}
