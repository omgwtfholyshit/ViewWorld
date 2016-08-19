using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models
{
    /// <summary>
    /// 景点信息
    /// </summary>
    /// 

    [DataContract]
    public class Scenery
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        [Required]
        public int Id { get; set; }
        [DataMember]
        [Required]
        public string Name { get; set; }
        public GeoPoint Coordinate { get; set; }
        public Location Location { get; set; }
        //统计该景点热门度
        public int Popularity { get; set; }

    }
    public class GeoPoint
    {
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

    }
}
