using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Enum;

namespace ViewWorld.Core.Models.BusinessModels
{
    public class BusinessOrder
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        /// <summary>
        /// UserFriendlyId
        /// </summary>
        [BsonRequired]
        public string OrderId { get; set; }
        [BsonRequired]
        public string UserId { get; set; }
        /// <summary>
        /// Item Related to the order to prevent multiple order with same item
        /// </summary>
        [BsonRequired]
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ContactNumber { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OrderedAt { get; set; } = DateTime.Now;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastModifiedAt { get; set; } = DateTime.Now;
        //行程起始日期。(PaymentReceived&&DateTime.Now)>CommenceDate可以评论。(PaymentReceived&&DateTime.Now.AddDays(7))<CommenceDate可以退款
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CommenceDate { get; set; }

        //行程结束日期。(PaymentReceived&&DateTime.Now)>FinishDate.AddDays(14)关闭评论。(PaymentReceived&&DateTime.Now)>=>OrderCompleted
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FinishDate { get; set; }
        public string ModificatorId { get; set; }
        public string ModificatorName { get; set; }
        public string SalesId { get; set; }
        public string SalesName { get; set; }
        public ProductType Type { get; set; }
        public OrderStatus Status { get; set; }

        public double Price { get; set; }
        public string PaymentSource { get; set; }
    }

}
