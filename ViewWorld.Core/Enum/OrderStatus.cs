using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Enum
{
    public enum OrderStatus
    {
        新创建订单 = 0,//Pending to confirm
        行程已确认,//Waiting for customers to pay
        现金收讫,//Waiting for the journey to start.
        现金已付给供应商,
        订单已完成,
        订单已关闭,
        订单已删除
    }

}
