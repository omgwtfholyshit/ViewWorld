using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Services.Trips
{
    public interface ITripService : ICRUDable<TripArrangement>
    {
        Task<Result> TestMethod();
        Task<Result> RetrieveTripArrangementById(string tripId);
        Task<Result> UpdateTripPartial(string tripId, CommonInfo data);
        Task<Result> UpdateTripPartial(string tripId, ProductInfo data);
        Task<Result> UpdateTripPartial(string tripId, List<Schedule> data);
        Task<Result> UpdateTripPartial(string tripId, TripPlan data);
        Task<Result> UpdateTripPartial(string tripId, TripProperty data);
        Task<Result> UploadPhoto(HttpRequestBase request);
        Task<Result> DeletePhotoById(string tripId,string photoId);
    }
}
