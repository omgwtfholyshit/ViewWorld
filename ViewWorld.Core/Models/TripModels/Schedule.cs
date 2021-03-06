﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.TripModels
{
    /// <summary>
    /// 日程信息
    /// </summary>
    public class Schedule
    {
        [BsonRequired]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 日程里面去往景点的细节
        /// </summary>
        public List<ScheduleItem> Details { get; set; }
        /// <summary>
        /// 用餐安排
        /// </summary>
        public string Meal { get; set; }
        public string Accommodation { get; set; }
        public string GroupPickUp { get; set; }
        public string PickUp { get; set; }
        public string Introduction { get; set; }
        //At Day 1 or 2 or whatever
        public int Day { get; set; }
    }
    public class ScheduleItem
    {
        public string Id { get; set; }
        public string Sceneries { get; set; }
        public string ActivityTime { get; set; }
        public string Arrangement { get; set; }
        public string Memo { get; set; }
    }
}
