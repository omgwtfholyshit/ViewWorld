using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using ViewWorld.Models.Trip;

namespace ViewWorld.Core.Models
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
        [JsonProperty("id")]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public GeoPoint Coordinate { get; set; }
        public Region Region { get; set; }
        //统计该景点热门度
        public string Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Modificator { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string Initial { get; set; }
        public int Popularity { get; set; }

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
