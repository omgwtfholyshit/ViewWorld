using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ViewWorld.Core.Models.TripModels;

namespace ViewWorld.Core.Models.ProviderModels
{
    public class Provider
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CommissionRate { get; set; }
        public string AwardRatio { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; } = false;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedDate { get; set; }
        public List<StartingPoint> StartingPoints { get; set; }
    }
}