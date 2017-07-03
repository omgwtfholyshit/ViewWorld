using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Core.Models.ViewModels;

namespace ViewWorld.Services.Trips
{
    public interface ITripService : ICRUDable<TripArrangement>
    {
        Task<Result> RetrieveTripArrangementById(string tripId);
        Task<IEnumerable<TripArrangement>> RetrieveTripArrangementBySearchModel(FinderViewModels model);
        Task<GetManyResult<TripArrangement>> RetrieveTripArrangementByFilter(FinderViewModels model);
        Task<Result> UpdateTripPartial(string tripId, CommonInfo data);
        Task<Result> UpdateTripPartial(string tripId, ProductInfo data);
        Task<Result> UpdateTripPartial(string tripId, List<Schedule> data);
        Task<Result> UpdateTripPartial(string tripId, List<TripPlan> data);
        Task<Result> UpdateTripPartial(string tripId, TripProperty data);
        Task<Result> UploadPhoto(HttpRequestBase request);
        Task<Result> SetFrontCover(string tripId, string photoId);
        Task<Result> DeletePhotoById(string tripId,string photoId);
        Task<Result> ToggleTripArrangement(string tripId);
        Task<Result> DisplayTripOnFrontPage(string tripId);
        Task<Result> CopyTripArrangement(string tripId);
        Task<string> CalculateTripPrice(List<PeoplePerRoomViewModel> rooms, DateTime departDate, string tripId, string planId);
    }
}
