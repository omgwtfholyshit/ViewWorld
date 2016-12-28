using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Sceneries
{
    public interface ISceneryService
    {
        Task<Result> AddScenery(Scenery model);
        Task<Result> DeleteScenery(string id);
        Task<Result> UpdateScenery(Scenery model);
        /// <summary>
        /// 更新本地图片
        /// </summary>
        /// <param name="photoList"></param>
        /// <returns></returns>
        Task<Result> UpdatePhotos(List<string> photoList);
        Task<GetListResult<Scenery>> SearchSceneries(string keyword);
    }
}
