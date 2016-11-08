using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

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
        [BsonRequired]
        public string Id { get; set; }
        [BsonRequired]
        public string Name { get; set; }
        [BsonRequired]
        public string EnglishName { get; set; }
        [BsonRequired]
        public string Initial { get; set; }
        [BsonRequired]
        public int SortOrder { get; set; }
        [BsonRequired]
        public bool IsSubRegion { get; set; }
        //是否前台可见
        [BsonRequired]
        public bool IsVisible { get; set; }
        public string ParentRegionId { get; set; }
        public List<Region> SubRegions {
            get {
                return _subRegions ?? new List<Region>();
            }
            set {
                _subRegions = value;
            }
        }
        [NotMapped]
        private List<Region> _subRegions;

    }
}