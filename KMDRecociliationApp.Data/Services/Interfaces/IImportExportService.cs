using KMDRecociliationApp.Domain.DTO.InsurerData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services.Interfaces
{
    public interface IImportExportService
    {
        Task<ImportResult> ImportApplicantsAsync(DataTable dataTable);
        Task<(byte[] fileContent, string contentType, string fileName)> ExportApplicantsAsync(
            List<ApplicantDto> applicants,
            string format);
    }

    public class ImportResult
    {
        public int ProcessedRecords { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
