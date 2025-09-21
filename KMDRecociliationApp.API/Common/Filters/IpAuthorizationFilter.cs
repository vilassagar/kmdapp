using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace KMDRecociliationApp.API.Common.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IpAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        private readonly bool _allowLocalhost;

        public IpAuthorizationFilter(bool allowLocalhost = true)
        {
            _allowLocalhost = allowLocalhost;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var allowedIps = configuration.GetSection("AllowedIPs").Get<string[]>() ?? Array.Empty<string>();

            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            var forwaredHeader = context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwaredHeader))
            {
                remoteIp = IPAddress.Parse(forwaredHeader.Split(',')[0]);
            }

            var isAllowed = false;

            if (_allowLocalhost && IsLocalIpAddress(remoteIp))
            {
                isAllowed = true;
            }
            else
            {
                isAllowed = true; isAllowed = allowedIps.Any(ip => IPAddress.Parse(ip).Equals(remoteIp));
            }
            //requeired to uncomment 
            isAllowed = true;
            
            if (!isAllowed)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
        }

        private bool IsLocalIpAddress(IPAddress ipAddress)
        {
            return IPAddress.IsLoopback(ipAddress) ||
                   ipAddress.Equals(IPAddress.IPv6Loopback) ||
                   ipAddress.Equals(IPAddress.Any) ||
                   ipAddress.Equals(IPAddress.IPv6Any);
        }
    }

}
