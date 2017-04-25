using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewWorld.Models
{
    public class FinderViewModels
    {
        public string keyword { get; set; }
        public string Region { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public int Days { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Sceneries { get; set; }

    }
}