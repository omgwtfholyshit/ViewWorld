using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Enum
{
    public class TripTypes
    {
        public enum PlanType
        {
            天天发团,
            指定日期发团,
            定期发团
        }
        public enum TripInfoType
        {
            通用信息,
            产品概要,
            单日行程,
            发团属性,
            发团计划
        }
    }
    
}
