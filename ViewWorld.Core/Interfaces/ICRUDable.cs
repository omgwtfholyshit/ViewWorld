using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;

namespace ViewWorld.Core.Interfaces
{
    public interface ICRUDable<T> where T : class, new()
    {
        Task<Result> AddEntity(T Entity);
        Task<GetListResult<T>> RetrieveEntitiesByKeyword(string keyword);
        Task<Result> UpdateEntity(T Entity);
        Task<Result> DeleteEntityById(string id);
    }
}
