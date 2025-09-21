using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivityMonitoringService.Configuration;
using UserActivityMonitoringService.Data;
using UserActivityMonitoringService.Model;

namespace UserActivityMonitoringService.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly UserActivityOptions _options;
        private readonly ILogger<UserActivityService> _logger;
        private readonly UserActivityDbContext _userActivityDbContext;
        public UserActivityService(IOptions<UserActivityOptions> options
            , ILogger<UserActivityService> logger, UserActivityDbContext userActivityDbContext)
        {
            _options = options.Value;
            _logger = logger;
            _userActivityDbContext= userActivityDbContext;
        }

        public async Task LogActivityAsync(UserActivity activity)
        {
            try
            {

                await _userActivityDbContext.UserActivities.AddAsync(activity);
                await _userActivityDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging user activity");
            }
        }
    }

}
