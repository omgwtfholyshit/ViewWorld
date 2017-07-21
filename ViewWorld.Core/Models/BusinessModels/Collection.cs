using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Enum;

namespace ViewWorld.Core.Models.BusinessModels
{
    /// <summary>
    /// 用户收藏
    /// </summary>
    [DataContract]
    public class Collection
    {
        [DataMember]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        [DataMember]
        [BsonRequired]
        public string UserId { get; set; }
        [DataMember]
        public string ItemId { get; set; }
        [DataMember]
        public string ItemName { get; set; }
        [DataMember]
        public string Memo { get; set; }
        [DataMember]
        public ProductType Type { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CollectedAt { get; set; }
    }
}
