using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Models.Trip
{
    /// <summary>
    /// 起始位置
    /// </summary>
    public class StartingPoint
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        [Required]
        public string Landmark { get; set; }
        //出发时间
        public string DepartTime { get; set; }
        //1,2,3,4,5,6,7 代表周一到周日
        public string AvailableDays { get; set; }
        public string Address { get; set; }
        public string ProviderName { get; set; }
        public string ProviderAlias { get; set; }
        public string ProviderId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        //管理员用户名
        public string UpdatedBy { get; set; }
        //统计该起点的热门度
        public int Popularity { get; set; }
        public bool IsArchived { get; set; } = false;
    }
}
