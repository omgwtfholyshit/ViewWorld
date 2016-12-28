using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.ProviderModels;

namespace ViewWorld.Services.Providers
{
    public interface IProviderService : ICRUDable<Provider>
    {
        Task<GetManyResult<Provider>> ListProviders();

    }
}
