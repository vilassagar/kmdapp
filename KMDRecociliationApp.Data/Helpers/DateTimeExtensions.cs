using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Helpers
{
    public static class DateTimeExtensions
    {
        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return (date.Date >= startDate.Date || date.Date <= startDate.Date) && date.Date <= endDate.Date;
        }
    }
}
