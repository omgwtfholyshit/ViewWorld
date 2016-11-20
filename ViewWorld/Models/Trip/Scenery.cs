using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using ViewWorld.Models.Trip;

namespace ViewWorld.Models.Trip
{
    /// <summary>
    /// 景点信息
    /// </summary>
    /// 
    public class Scenery
    {
        public Scenery()
        {
            PublishedAt = DateTime.Now;
            LastUpdateAt = DateTime.Now;
            Popularity = 0;
            Publisher = "Admin";
            Modificator = "Admin";
        }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public GeoPoint Coordinate {
            get
            {
                return _coord ?? new GeoPoint();
            }
            set
            {
                _coord = value;
            }
        }
        public string RegionId { get; set; }
        public string ParentRegionId { get; set; }
        public string Address { get; set; }
        public string Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Modificator { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string Initial { get; set; }
        //统计该景点热门度
        public int Popularity { get; set; }
        private GeoPoint _coord { get; set; }

    }
    public class GeoPoint
    {
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

    }
}
