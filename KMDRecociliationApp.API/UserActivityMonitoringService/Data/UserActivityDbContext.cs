using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivityMonitoringService.Model;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Data;
using EntityFrameworkCore.UseRowNumberForPaging;
namespace UserActivityMonitoringService.Data
{
    public class UserActivityDbContext : DbContext
    {
          
        private readonly IConfiguration _configuration;
  
        public UserActivityDbContext(DbContextOptions<UserActivityDbContext> options
            , IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }
       
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptions)
     => dbContextOptions
          .UseSqlServer(_configuration.GetConnectionString("UserActivityConnection")
         , builder => builder.UseRowNumberForPaging()).EnableSensitiveDataLogging();

        public DbSet<UserActivity> UserActivities { get; set; }
    }

}
