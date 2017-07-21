using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Enum
{
    public enum OrderStatus
    {
        Created,//Pending to confirm
        Confirmed,//Waiting for customers to pay
        PaymentReceived,//Waiting for the journey to start.
        PaymentSentToProvider,
        Completed,
        Closed,
        Deleted
    }

}
