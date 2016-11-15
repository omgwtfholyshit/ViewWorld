using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Enum;

namespace ViewWorld.Core.Models
{
    /// <summary>
    /// 行程信息
    /// </summary>
    /// 
    public class Trip
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        public string Name { get; set; }
        public CommonInfo CommonInfo { get; set; }
        public ProductInfo ProductionInfo { get; set; }
        public List<Schedule> Schedules { get; set; }
        public TripPlan TripPlan { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CommonInfo
    {
        public string ProviderPrefix { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string Promotion { get; set; }
        public string Theme { get; set; }
        //1,2,3,4,5,6,7 代表周一到周日
        public string AvailableDate { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public decimal Price { get; set; }
        public bool UsePoints { get; set; }
        public string Introduction { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public List<Dictionary<string,string>> SelfPayActivities { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
    }
    /// <summary>
    /// 产品概要
    /// </summary>
    public class ProductInfo
    {
        public StartingPoint DepartingCity { get; set; }
        public StartingPoint ArrivingCity { get; set; }
        /// <summary>
        /// 途经景点
        /// </summary>
        public List<Scenery> Sceneries { get; set; }
        /// <summary>
        /// 行程天数
        /// </summary>
        public int TotalDays { get; set; }
        /// <summary>
        /// 行程特色
        /// </summary>
        public string Feature { get; set; }
        /// <summary>
        /// 行程介绍
        /// </summary>
        public string Intro { get; set; }

    }
    //发团计划
    public class TripPlan
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        public TripPlanType Type { get; set; }
        public bool OneDayOnly { get; set; }
        public DateTime AvaiableFrom { get; set; }
        public DateTime AvaiableTo { get; set; }
        public decimal RoomPrice { get; set; }
        public CurrencyType CurrencyType { get; set; }
    }
}
