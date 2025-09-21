using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ConfigurationModels
{
    public record class CaptchaConfiguration
    {
        public string ?SecretKey { get; set; }
    }
}
