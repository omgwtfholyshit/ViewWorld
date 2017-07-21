using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewWorld.Core.Models.TripModels
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
        public List<string> Photos
        {
            get
            {
                return _photos ?? new List<string>();
            }
            set
            {
                _photos = value;
            }
        }
        public string RegionId { get; set; }
        public string ParentRegionId { get; set; }
        public string Address { get; set; }
        public string Publisher { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublishedAt { get; set; }
        public string Modificator { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdateAt { get; set; }
        public string Initial { get; set; }
        public decimal ExtraCost { get; set; } = 0;
        //统计该景点热门度
        public int Popularity { get; set; }
        public string Description { get; set; }
        private GeoPoint _coord { get; set; }
        private List<string> _photos { get; set; }
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
