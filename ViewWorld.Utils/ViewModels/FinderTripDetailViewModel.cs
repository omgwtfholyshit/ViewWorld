using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Utils.ViewModels
{
    public class FinderTripDetailViewModel
    {
        public TripArrangement Trip { get; set; }
        public string DepartCity { get; set; }
        public string ArrivalCity { get; set; }
        public string FirstAvaiableDate { get; set; }
        public string TripData { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }

    }
}
