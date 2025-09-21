using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Common
{
    // Method to convert string to enum
    public static class EnumConverter
    {
        // Generic method to convert string to any enum
        public static T ConvertToEnum<T>(string value) where T : System.Enum
        {
            // Try to parse directly (case-sensitive)
            if (System.Enum.TryParse(typeof(T), value, out object result))
            {
                return (T)result;
            }

            // Try case-insensitive parsing
            if (System.Enum.TryParse(typeof(T), value, true, out object caseInsensitiveResult))
            {
                return (T)caseInsensitiveResult;
            }

            return default; // Return default value if parsing fails
        }
    }
}
