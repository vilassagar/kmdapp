using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivityMonitoringService.Model
{
    public class UserActivity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestPath { get; set; }
        public string? QueryString { get; set; }
        public string? Method { get; set; }
        public string? RequestBody { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserAgent { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
