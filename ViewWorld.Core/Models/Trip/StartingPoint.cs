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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public GeoPoint Coordinate { get; set; }
        public Location Location { get; set; }
        //统计该起点的热门度
        public int Popularity { get; set; }
    }
}
