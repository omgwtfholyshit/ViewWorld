using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Models
{
    public class GlobalSetting
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Label { get; set; }
        public string EnglishLabel { get; set; }
        public string Value { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> HistoryValue { get; set; }
    }
}