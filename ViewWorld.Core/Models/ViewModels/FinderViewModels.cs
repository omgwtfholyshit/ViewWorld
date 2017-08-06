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
        public string FinishCity { get; set; } = "不限";
        public int Days { get; set; }
        public string Theme { get; set; }
        /// <summary>
        /// True时只显示该属性为true时的行程，false显示所有行程
        /// </summary>
        public bool DisplayOnFrontPageTripsOnly { get; set; } = false;

    }
    public class PeoplePerRoomViewModel
    {
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}