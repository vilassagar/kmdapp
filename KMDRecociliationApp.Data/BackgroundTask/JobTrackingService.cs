using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.BackgroundTask
{
    public class JobStatus
    {
        public string JobId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
    }
    // JobTrackingService.cs
    public interface IJobTrackingService
    {
        Task CreateJobAsync(string jobId, string email, string fileName);
        Task UpdateJobStatusAsync(string jobId, string status, int? processedRecords = null);
        Task<JobStatus> GetJobStatusAsync(string jobId);
    }

    public class JobTrackingService : IJobTrackingService
    {
        private readonly DbContext _dbContext;

        public JobTrackingService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateJobAsync(string jobId, string email, string fileName)
        {
            var jobStatus = new JobStatus
            {
                JobId = jobId,
                Status = "Queued",
                CreatedAt = DateTime.UtcNow,
                Email = email,
                FileName = fileName
            };

            _dbContext.Set<JobStatus>().Add(jobStatus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateJobStatusAsync(string jobId, string status, int? processedRecords = null)
        {
            var job = await _dbContext.Set<JobStatus>().FindAsync(jobId);
            if (job != null)
            {
                job.Status = status;
                if (processedRecords.HasValue)
                    job.ProcessedRecords = processedRecords.Value;
                if (status == "Completed")
                    job.CompletedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<JobStatus> GetJobStatusAsync(string jobId)
        {
            return await _dbContext.Set<JobStatus>().FindAsync(jobId);
        }
    }

}
