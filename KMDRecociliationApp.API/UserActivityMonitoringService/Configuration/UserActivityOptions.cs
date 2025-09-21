using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivityMonitoringService.Configuration
{
    public class UserActivityOptions
    {
        public bool IsEnabled { get; set; } = true;  // Default to enabled
        public string ConnectionString { get; set; }

        public bool LogRequestBody { get; set; } = true;
        public bool LogQueryString { get; set; } = true;
        public int MaxRequestBodyLength { get; set; } = 1000;
        public string[] ExcludePaths { get; set; } = Array.Empty<string>();
        public string[] ExcludeContentTypes { get; set; } = Array.Empty<string>();
    }
}
