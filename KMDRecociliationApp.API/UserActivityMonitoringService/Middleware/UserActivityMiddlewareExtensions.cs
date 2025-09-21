using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivityMonitoringService.Configuration;
using UserActivityMonitoringService.Services;

namespace UserActivityMonitoringService.Middleware
{
    public static class UserActivityMiddlewareExtensions
    {
        public static IServiceCollection AddUserActivity(
            this IServiceCollection services,
            Action<UserActivityOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddScoped<IUserActivityService, UserActivityService>();
            return services;
        }

        public static IApplicationBuilder UseUserActivity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserActivityMiddleware>();
        }
    }
}
