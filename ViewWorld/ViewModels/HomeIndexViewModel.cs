using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.ViewModels
{
    public class HomeIndexViewModel
    {
        public string BackgroundUrl {
            get
            {
                var rand = (new Random()).Next(1, 12);
                return "/Images/Background/background_" + rand + ".jpg";
            }
        }
        public IEnumerable<TripArrangement> RecommendationList { get; set; }
    }
}