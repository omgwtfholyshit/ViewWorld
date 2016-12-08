using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.ProviderModels;

namespace ViewWorld.Services.Providers
{
    public interface IProviderService
    {
        Task<Result> AddProvider(Provider model);
        Task<Result> DeleteProvider(string id);
        Task<Result> EditProvider(Provider model);
        Task<GetManyResult<Provider>> ListProviders();

    }
}
