using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Sceneries
{
    public interface ISceneryService : ICRUDable<Scenery>
    {
        
        /// <summary>
        /// 更新本地图片
        /// </summary>
        /// <param name="photoList"></param>
        /// <returns></returns>
        Task<Result> UpdatePhoto(List<string> photoList);
        Task<Result> UploadPhoto(HttpFileCollectionBase files, string id);
        Task<List<string>> ListPhotos(string id);
        Task<GetOneResult<Scenery>> RetrieveEntitiesById(GetListResult<Scenery> cachedData, string id);
        Task<GetListResult<Scenery>> RetrieveEntitiesByKeyword(GetListResult<Scenery> cachedData, string keyword);
    }
}
