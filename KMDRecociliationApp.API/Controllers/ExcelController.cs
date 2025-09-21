using KMDRecociliationApp.Data.BackgroundTask;
using KMDRecociliationApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using IEmailService = KMDRecociliationApp.Data.BackgroundTask.IEmailService;
using EmailService = KMDRecociliationApp.Data.BackgroundTask.EmailService;
namespace KMDRecociliationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IExcelProcessingService _excelProcessingService;
        private readonly IJobTrackingService _jobTrackingService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExcelController> _logger;

        public ExcelController(
            IBackgroundTaskQueue taskQueue,
            IExcelProcessingService excelProcessingService,
            IJobTrackingService jobTrackingService,
            IEmailService emailService,
            ILogger<ExcelController> logger)
        {
            _taskQueue = taskQueue;
            _excelProcessingService = excelProcessingService;
            _jobTrackingService = jobTrackingService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
             string email="vilass@";
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email is required");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest("Invalid file format. Only Excel files are allowed.");

                var jobId = Guid.NewGuid().ToString();
               // await _excelProcessingService.ProcessExcelFileAsync(file, jobId);

                // Create job tracking record
                await _jobTrackingService.CreateJobAsync(jobId, email, file.FileName);

                // Queue the background work
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    try
                    {
                        await _jobTrackingService.UpdateJobStatusAsync(jobId, "Processing");
                        await _excelProcessingService.ProcessExcelFileAsync(file, jobId);
                        await _jobTrackingService.UpdateJobStatusAsync(jobId, "Completed");
                        await _emailService.SendCompletionEmailAsync(email, jobId, file.FileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing job {jobId}");
                        await _jobTrackingService.UpdateJobStatusAsync(jobId, "Failed");
                    }
                    finally
                    {
                        //if (File.Exists(filePath))
                        //{
                        //    File.Delete(filePath);
                        //}
                    }
                });

                return Ok(new
                {
                    jobId,
                    message = "File uploaded successfully and processing has started",
                    statusEndpoint = $"/api/excel/status/{jobId}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file upload");
                return StatusCode(500, "An error occurred while uploading the file");
            }
        }

        [HttpGet("status/{jobId}")]
        public async Task<IActionResult> GetJobStatus(string jobId)
        {
            var status = await _jobTrackingService.GetJobStatusAsync(jobId);
            if (status == null)
                return NotFound("Job not found");

            return Ok(status);
        }
    }

}
