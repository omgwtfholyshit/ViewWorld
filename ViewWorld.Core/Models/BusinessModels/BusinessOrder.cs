using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        public string ContactName { get; set; }
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
        //销售人员
        public string SalesId { get; set; }
        public string SalesName { get; set; }
        //供应商
        public string ProviderName { get; set; }
        public ProductType Type { get; set; }
        public OrderStatus Status { get; set; }
        //json string
        public string OrderDetail { get; set; }
        public string SelfChooseActivities { get; set; }
        public double Price { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string PaymentSource { get; set; }
        public string PaymentId { get; set; }
        public string ThirdPartyPaymentId { get; set; }
        [NotMapped]
        public string OrderStatus { get { return this.Status.ToString(); } }
        [NotMapped]
        public string OrderType { get { return this.Type.ToString(); } }
        [NotMapped]
        public string OrderCurrency { get { return this.CurrencyType.ToString(); } }
    }

}
