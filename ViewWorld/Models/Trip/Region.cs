using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ViewWorld.Models.Trip
{
    public class Region
    {
        public Region()
        {
            SortOrder = 0;
            IsSubRegion = false;
            IsVisible = true;
        }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Initial { get; set; }
        public int SortOrder { get; set; }
        public bool IsSubRegion { get; set; }
        //是否前台可见
        public bool IsVisible { get; set; }
        public string ParentRegionId { get; set; }
        public List<Region> SubRegions { get; set; }

    }
}