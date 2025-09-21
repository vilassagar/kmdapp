using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.BackgroundTask
{
    // BackgroundExcelService.cs
    public class BackgroundExcelService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<BackgroundExcelService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BackgroundExcelService(
            IBackgroundTaskQueue taskQueue,
            ILogger<BackgroundExcelService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing Excel file.");
                }
            }
        }
    }

    // BackgroundTaskQueue.cs
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);
        ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public BackgroundTaskQueue(int capacity = 100)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem)
        {
            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
    // ExcelProcessingService.cs
    public interface IExcelProcessingService
    {
        Task ProcessExcelFileAsync(IFormFile file, string jobId);
    }

    public class ExcelProcessingService : IExcelProcessingService
    {
        private readonly ILogger<ExcelProcessingService> _logger;
        private readonly DbContext _dbContext;

        public ExcelProcessingService(ILogger<ExcelProcessingService> logger, DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task ProcessExcelFileAsync(IFormFile file, string jobId)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            var rowCount = worksheet.Dimension.Rows;
            var batchSize = 1000;
           // var records = new List<YourDataModel>();

            //for (int row = 2; row <= rowCount; row++)
            //{
            //    var record = new YourDataModel
            //    {
            //        // Map your Excel columns to model properties
            //        Column1 = worksheet.Cells[row, 1].Value?.ToString(),
            //        Column2 = worksheet.Cells[row, 2].Value?.ToString(),
            //        // Add more mappings as needed
            //    };
            //    records.Add(record);

            //    if (records.Count >= batchSize || row == rowCount)
            //    {
            //        await _dbContext.BulkInsertAsync(records);
            //        records.Clear();
            //        _logger.LogInformation($"Processed batch for job {jobId}: Row {row}/{rowCount}");
            //    }
            //}
        }
    }

}
