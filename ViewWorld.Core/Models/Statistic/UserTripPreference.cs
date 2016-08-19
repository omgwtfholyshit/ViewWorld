using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.Statistic
{
    /// <summary>
    /// 用于记录用户的偏好旅行记录
    /// </summary>
    public class UserTripPreference
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }

    }
}
