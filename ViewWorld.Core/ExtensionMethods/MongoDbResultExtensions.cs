using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;

namespace ViewWorld.Core.ExtensionMethods
{
    public static class MongoDbResultExtensions
    {
        public static GetListResult<T> ManyToListResult<T>(this GetManyResult<T> manyResult) where T : class, new()
        {
            var listResult = new GetListResult<T>();
            listResult.Message = manyResult.Message;
            listResult.Success = manyResult.Success;
            listResult.Entities = manyResult.Entities.ToList();
            return listResult;
        }
    }
}
