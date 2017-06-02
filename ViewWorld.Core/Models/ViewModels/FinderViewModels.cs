using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewWorld.Core.Models.ViewModels
{
    public class FinderViewModels
    {
        public string keyword { get; set; }
        public string Region { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public int Days { get; set; }
        public string Theme { get; set; }

    }
    public class PeoplePerRoomViewModel
    {
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}