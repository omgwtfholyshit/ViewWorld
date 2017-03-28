using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.TripModels
{
    public struct HotelPrice
    {
        public string Name { get; set; }
        public decimal SinglePrice { get; set; }
        public decimal DoublePrice { get; set; }
        public decimal TriplePrice { get; set; }
        public decimal QuadplexPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public decimal RoomDifference { get; set; }//单房差
    }
}
