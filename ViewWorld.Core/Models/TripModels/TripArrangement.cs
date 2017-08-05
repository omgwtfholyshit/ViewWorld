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

namespace ViewWorld.Core.Models.TripModels
{
    /// <summary>
    /// 行程信息
    /// </summary>
    /// 
    [Serializable]
    public class TripArrangement
    {
        public TripArrangement(CommonInfo cInfo, ProductInfo pInfo, List<Schedule> schedules, List<TripPlan> tplan,TripProperty tproperty)
        {
            this.CommonInfo = cInfo;
            this.ProductInfo = pInfo;
            this.Schedules = schedules;
            this.TripPlans = tplan;
            this.TripProperty = tproperty;
        }
        public TripArrangement() { }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Publisher { get; set; }
        public string PublisherId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublishedAt { get; set; }
        public int Popularity { get; set; } = 0;
        public int TripOrdered { get; set; } = 0;
        public int SortOrder { get; set; } = 0;
        
        public bool DisplayOnFrontPage = false;
        public CommonInfo CommonInfo {
            get
            {
                return _commonInfo ?? new CommonInfo();
            }
            set
            {
                _commonInfo = value;
            }
        }
        public ProductInfo ProductInfo
        {
            get
            {
                return _productInfo ?? new ProductInfo();
            }
            set
            {
                _productInfo = value;
            }
        }
        public List<Schedule> Schedules {
            get
            {
                return _scheduleList ?? new List<Schedule>();
            }
            set
            {
                _scheduleList = value;
            }
        }
        public TripProperty TripProperty {
            get
            {
                return _tripProperty ?? new TripProperty();
            }
            set
            {
                _tripProperty = value;
            }
        }
        public List<TripPlan> TripPlans {
            get
            {
                return _tripPlan ?? new List<TripPlan>();
            }
            set
            {
                _tripPlan = value;
            }
        }
        public bool IsVisible { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        #region private properties
        CommonInfo _commonInfo { get; set; }
        ProductInfo _productInfo { get; set; }
        List<Schedule> _scheduleList { get; set; }
        List<TripPlan> _tripPlan { get; set; }
        TripProperty _tripProperty { get; set; }
        #endregion

    }

    public class CommonInfo
    {
        public class PhotoInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string FileLocation { get; set; }
        }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string GroupId { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        /// <summary>
        /// 0.出发地参团|1.目的地参团|2.自由行|3.目的地自由行|4.游轮|5.游学|6.私人定制|7.机票|
        /// </summary>
        public string TripType { get; set; } = "";
        /// <summary>
        /// 优惠政策。a.特价 b.限时促销 c.热卖 d.买二送二 e.买二送一 f.推荐 g.积分优惠 h.免费接机
        /// </summary>
        public string Promotion { get; set; }
        /// <summary>
        /// 主题。a.亲子游 b.自然风光 c.主题公园 d.都市名城 e.冒险之旅 f.毕业旅行 g.蜜月之旅 h.时尚购物 i.商务之旅 j.父母游 k.假期特惠 l.自由行 m.新年特惠 n.特色游
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 1,2,3,4,5,6,7 代表周一到周日
        /// </summary>
        public string AvailableDates { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string PriceType {
            get {
                var priceType = "";
                switch (CurrencyType)
                {
                    case CurrencyType.美元:
                        priceType = "USD$";
                        break;
                    case CurrencyType.澳元:
                        priceType = "AUD$";
                        break;
                    case CurrencyType.欧元:
                        priceType = "EUR€";
                        break;
                    case CurrencyType.人民币:
                        priceType = "RMB￥";
                        break;
                }
                return priceType;
            }
        }
        public string ShortPriceType
        {
            get
            {
                var priceType = "";
                switch (CurrencyType)
                {
                    case CurrencyType.美元:
                        priceType = "$";
                        break;
                    case CurrencyType.澳元:
                        priceType = "$";
                        break;
                    case CurrencyType.欧元:
                        priceType = "€";
                        break;
                    case CurrencyType.人民币:
                        priceType = "￥";
                        break;
                }
                return priceType;
            }
        }
        public decimal LowestPrice { get; set; }
        public bool UsePoints { get; set; } = true;
        public int Points { get; set; }
        public string Introduction { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public List<string> SelfPayActivities {
            get
            {
                return _selfpayAct ?? new List<string>();
            }
            set
            {
                _selfpayAct = value;
            }
        }
        public string Keyword { get; set; }
        /// <summary>
        /// 用作自费项目前的介绍
        /// </summary>
        public string Description { get; set; }

        public PhotoInfo FrontCover { get; set; }
        public List<PhotoInfo> Photos {
            get
            {
                return _photoList ?? new List<PhotoInfo>();
            }
            set
            {
                _photoList = value;
            }
        }
        List<PhotoInfo> _photoList { get; set; }
        List<string> _selfpayAct { get; set; }
    }
    /// <summary>
    /// 产品概要
    /// </summary>
    public class ProductInfo
    {
        public string DepartingCity { get; set; }
        public string ArrivingCity { get; set; }
        public string FinishingCity { get; set; }
        /// <summary>
        /// 途经景点
        /// </summary>
        public string Sceneries { get; set; }
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
    public class TripProperty
    {
        //public class AirportPickUp
        //{
        //    public bool IsFree { get; set; } = false;
        //    public DateTime PickUpStartAt { get; set; }
        //    public DateTime PickUpEndAt { get; set; }
        //    public decimal Price { get; set; } = 0;
        //    public string Title { get; set; }

        //}
        //public List<AirportPickUp> PickUpInfos {
        //    get
        //    {
        //        return _pickupInfo ?? new List<AirportPickUp>();
        //    }
        //    set
        //    {
        //        _pickupInfo = value;
        //    }
        //}
        public List<HotelPrice> HotelPrices
        {
            get
            {
                return _priceList ?? new List<HotelPrice>();
            }
            set
            {
                _priceList = value;
            }
        }
        
        public string DepartingLocation { get; set; }
        public List<string> SelectableRoutes {
            get
            {
                return _routes ?? new List<string>();
            }
            set
            {
                _routes = value;
            }
        }
        public List<string> SelfChooseActivities {
            get
            {
                return _activities ?? new List<string>();
            }
            set
            {
                _activities = value;
            }
        }
        List<HotelPrice> _priceList { get; set; }
        //List<AirportPickUp> _pickupInfo { get; set; }
        List<string> _routes { get; set; }
        List<string> _activities { get; set; }
    }
    
    //发团计划
    public class TripPlan
    {
        public class TripPriceForSpecificDate
        {
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime TripDate { get; set; } 
            public HotelPrice BasePrice { get; set; }//原始价格
            
            
        }
        [BsonRequired]
        public string Id { get; set; }
        public TripTypes.PlanType Type { get; set; }
        public bool IsOneDayOnly { get; set; } = false;
        public bool IsRoomDiffApplied { get; set; } = false;
        public string SelectedDates { get; set; }
        public string WeekInfo { get; set; }
        public int RaisePriceByPercentage { get; set; }//提价
        public HotelPrice AdditionalPrice { get; set; }//房间加价
        public List<TripPriceForSpecificDate> TripPrices {
            get
            {
                return _tripPriceList ?? new List<TripPriceForSpecificDate>();
            }
            set
            {
                _tripPriceList = value;
            }
        }
        List<TripPriceForSpecificDate> _tripPriceList { get; set; }
    }
}
