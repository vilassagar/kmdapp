using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Common
{
    public class CommonHelper
    {
        public static bool IsDateInRange(DateTime currentDate, DateTime startDate, DateTime endDate)
        {
            // Normalize all dates to start of day to ignore time component
            currentDate = currentDate.Date;
            startDate = startDate.Date;
            endDate = endDate.Date;

            // Check if current date is between start and end dates (inclusive)
            return !(currentDate >= startDate && currentDate <= endDate);
        }

        public static bool CheckPolicyExists(int plicyTypeId)
        {
          return  (plicyTypeId == (int)ProductPolicyType.BasePolicy ||
                                   plicyTypeId == (int)ProductPolicyType.OPD ||
                                   plicyTypeId == (int)ProductPolicyType.Other ||
                                  plicyTypeId == (int)ProductPolicyType.AgeBandPremium ||
                                    plicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy);
        }
        public static int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;

            // Total months difference
            int age = (today.Year - dateOfBirth.Year);

            // Adjust if the day of the month hasn't occurred yet
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

    }
}
