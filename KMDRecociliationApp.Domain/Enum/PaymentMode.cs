using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Enum
{
    public enum PaymentMode
    {
        Online=1,Offline=2
    }
    public enum PaymentTypes
    {
        Cheque = 1, NEFT = 2,UPI=3, Gateway=4, Unknown = 0
    }
    public enum PaymentStatus
    {
        Pending = 0, Completed = 1, Rejected = 2, Initiated = 3, Failed = 4, Unknown=5,NotStarted=99
    }

}
