using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models
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
        public string Name { get; set; }
        //出发时间
        public string DepartAt { get; set; }
        //1,2,3,4,5,6,7 代表周一到周日
        public string AvailableDate { get; set; }
        public string Address { get; set; }
        //统计该起点的热门度
        public int Popularity { get; set; }
    }
}
