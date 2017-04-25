using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Cities
{
    public interface ICityService: ICRUDable<CityInfo>
    {
        Task<IEnumerable<IGrouping<string, CityInfo>>> GetCitiesByGroup(string intial, bool isChinsesCity);
    }
}
