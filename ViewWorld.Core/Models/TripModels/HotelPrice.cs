using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.TripModels
{
    public class HotelPrice
    {
        public string Name { get; set; }
        public double SinglePrice { get; set; }
        public double DoublePrice { get; set; }
        public double TriplePrice { get; set; }
        public double QuadplexPrice { get; set; }
        public double ChildPrice { get; set; }
        public double RoomDifference { get; set; } //单房差
    }
}
