using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivityMonitoringService.Model;

namespace UserActivityMonitoringService.Services
{
    public interface IUserActivityService
    {
        Task LogActivityAsync(UserActivity activity);
    }

}
