using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;
using ViewWorld.Models.Trip;
using ViewWorld.Utils;
using ViewWorld.Utils.ViewModels;

namespace ViewWorld.Models.Managers
{
    public class TripManager 
    {
        #region Constructor
        private MongoRepository Repo;
        public TripManager(MongoRepository _repo)
        {
            Repo = _repo;
        }
        public TripManager()
            :this(new MongoRepository())
        {

        }
        #endregion
        #region 区域管理
        public async Task<Result> AddRegion(Region model)
        {
            return await Repo.AddOne(model);
        }
        public async Task<Result> DeleteRegion(string id)
        {
            return await Repo.DeleteOne<Region>(id);
        }
        public async Task<Result> UpdateRegion(string id,Region model)
        {
            model.Id = id;
            return await Repo.ReplaceOne(id, model);
        }
        public async Task<Result> GetRegions()
        {
            return await Repo.GetAll<Region>();
        }
        public async Task<Result> SearchRegions(string keyword)
        {
            var builder = Builders<Region>.Filter;
            FilterDefinition<Region> filter; 
            if (keyword.Length == 1 && !Tools.isChineseLetter(keyword))
            {
                filter = builder.Where(r => r.Initial.ToUpper() == keyword.ToUpper());
            }else
            {
                filter = builder.Where(s => s.Name.Contains(keyword) || s.EnglishName.ToUpper().Contains(keyword.ToUpper()));
            }
            return await Repo.GetMany(filter);
        }
        #endregion
    }
}