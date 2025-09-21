using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using iText.Kernel.Pdf;
using iText.StyledXmlParser.Jsoup.Nodes;
using KMDRecociliationApp.Data.Repositories.Interfaces;
using KMDRecociliationApp.Data.Repositories.Interfaces.KMD.EnrolmentPortal.Repositories.Interfaces;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO.InsurerData;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;

namespace KMDRecociliationApp.Data.Services
{

    public class ImportExportService : IImportExportService
    {
        private readonly IApplicantRepository _applicantRepository;
        private readonly IBankDetailsRepository _bankDetailsRepository;
        private readonly IDependentRepository _dependentRepository;
        private readonly ILogger<ImportExportService> _logger;

        public ImportExportService(
            IApplicantRepository applicantRepository,
            IBankDetailsRepository bankDetailsRepository,
            IDependentRepository dependentRepository,
            ILogger<ImportExportService> logger)
        {
            _applicantRepository = applicantRepository;
            _bankDetailsRepository = bankDetailsRepository;
            _dependentRepository = dependentRepository;
            _logger = logger;
        }

        public async Task<ImportResult> ImportApplicantsAsync(DataTable dataTable)
        {


            return await ImportFromExcelAsync(dataTable);


        }


        private async Task<ImportResult> ImportFromExcelAsync(DataTable dataTable)
        {
            var result = new ImportResult();

            try
            {

                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {

                        // Extract data from Excel row
                        var record = new ApplicantImportDto
                        {
                            FirstName = row["First Name"].ToString(),
                            LastName = row["Last Name"].ToString(),
                            DateOfBirth = row["DOB"].ToString(),
                            Gender = row["Gender"].ToString(),
                            ContactNumber = row["Contact/Mobile No"].ToString(),
                            Email = row["Email"].ToString(),
                            Salary = row["Salary"].ToString(),
                            AssociatedOrganization = row["Associated Organization"].ToString(),
                            Address = row["Location/Address"].ToString(),
                            IdCardType = row["ID Card type"].ToString(),
                            IdCardNumber = row["ID Card no"].ToString(),
                            ProductName = row["Product Name"].ToString(),
                            DependentFirstName = row["Dependent First Name"].ToString(),
                            DependentLastName = row["Dependent Last Name"].ToString(),
                            DependentRelationship = row["Dependent Relationship"].ToString(),
                            DependentDateOfBirth = row["Dependent DOB"].ToString(),
                            BankName = row["Bank Name"].ToString(),
                            BankAccountNumber = row["Bank Account No"].ToString(),
                            BankIfscCode = row["Bank IFSC Code"].ToString(),
                            BankBranchDetails = row["Bank Branch name & Address"].ToString(),
                            MicrCode = row["MICR Code"].ToString(),
                        };



                        // Map to entity
                        var applicant = MapToEntity(record);
                        applicant.GenerateUniqueIdentifier();
                        var appobj = await _applicantRepository.CheckUnique(applicant.UniqueIdentifier);
                        if (appobj == null)
                            return new ImportResult();
                        // Map to entity
                        var bankdetails = MapBankDetailsEntity(record);
                        // Map to entity
                        var dependentdetails = MapDependentEntity(record);

                        // Add to database
                        await _applicantRepository.AddAsync(applicant);
                        await _applicantRepository.SaveChangesAsync();

                        // Add bank details if provided
                        if (bankdetails != null)
                        {
                            bankdetails.ApplicantId = applicant.Id;
                            await _bankDetailsRepository.AddAsync(bankdetails);
                            await _bankDetailsRepository.SaveChangesAsync();
                        }
                        // Add bank details if provided
                        if (dependentdetails != null)
                        {
                            dependentdetails.ApplicantId = applicant.Id;
                            await _dependentRepository.AddAsync(dependentdetails);
                            await _dependentRepository.SaveChangesAsync();
                        }


                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Error processing row {result.ProcessedRecords}: {ex.Message}");
                        _logger.LogError(ex, "Error processing Excel row");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error reading Excel file: {ex.Message}");
                _logger.LogError(ex, "Error reading Excel file");
            }

            return result;
        }

        private ApplicantInsurancePolicy MapToEntity(ApplicantImportDto dto)
        {

            var applicant = new ApplicantInsurancePolicy
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,

                Gender = EnumConverter.ConvertToEnum<Gender>(dto.Gender),
                MobileNumber = dto.ContactNumber,
                Salary = Convert.ToDecimal(dto.Salary),
                AssociatedOrganization = dto.AssociatedOrganization,
                Address = dto.Address,
                IdCardType = EnumConverter.ConvertToEnum<IdCardType>(dto.IdCardType),
                ProductName = dto.ProductName,
                IdCardNumber = dto.IdCardNumber,
                CreatedAt = DateTime.UtcNow
            };
            if (DateTime.TryParse(dto.DateOfBirth, out DateTime dateOfBirth))
            {
                applicant.DateOfBirth = dateOfBirth;
            }
            // Generate unique identifier
            applicant.GenerateUniqueIdentifier();



            return applicant;
        }

        private ApplicantBankDetails MapBankDetailsEntity(ApplicantImportDto dto)
        {


            // Create bank details
            if (!string.IsNullOrEmpty(dto.BankName) && !string.IsNullOrEmpty(dto.BankAccountNumber))
            {
                var bankDetails = new ApplicantBankDetails
                {
                    BankName = dto.BankName,
                    BankAccountNumber = dto.BankAccountNumber,
                    BankIfscCode = dto.BankIfscCode,
                    BankBranchDetails = dto.BankBranchDetails,
                    CreatedAt = DateTime.UtcNow
                };
                return bankDetails;
            }


            return null;
        }

        private ApplicantDependent MapDependentEntity(ApplicantImportDto dto)
        {


            if (!string.IsNullOrEmpty(dto.DependentFirstName)
                )
            {
                var dependents = new ApplicantDependent
                {

                    FirstName = dto.DependentFirstName,
                    LastName = dto.DependentLastName,
                    Relationship = dto.DependentRelationship,
                    //DateOfBirth = dto.DependentDateOfBirth,
                    CreatedAt = DateTime.UtcNow

                }; if (DateTime.TryParse(dto.DependentDateOfBirth, out DateTime dateOfBirth))
                {
                    dependents.DateOfBirth = dateOfBirth;
                }
                return dependents;

            }

            return null;
        }

        public async Task<(byte[] fileContent, string contentType, string fileName)> ExportApplicantsAsync(
            List<ApplicantDto> applicants,
            string format)
        {
            switch (format.ToLowerInvariant())
            {
                case "xlsx":
                    return await ExportToExcelAsync(applicants);
                case "csv":
                    return await ExportToCsvAsync(applicants);
                //case "pdf":
                //    return await ExportToPdfAsync(applicants);
                default:
                    throw new NotSupportedException($"Export format {format} is not supported");
            }
        }

        private async Task<(byte[] fileContent, string contentType, string fileName)> ExportToExcelAsync(List<ApplicantDto> applicants)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Applicants");

            // Add headers
            worksheet.Cell(1, 1).Value = "Full Name";
            worksheet.Cell(1, 2).Value = "Son/Daughter of";
            worksheet.Cell(1, 3).Value = "Date of Birth";
            worksheet.Cell(1, 4).Value = "Gender";
            worksheet.Cell(1, 5).Value = "Contact/Mobile No";
            worksheet.Cell(1, 6).Value = "Salary";
            worksheet.Cell(1, 7).Value = "Associated Organization";
            worksheet.Cell(1, 8).Value = "Location/Address";
            worksheet.Cell(1, 9).Value = "ID Card Type";
            worksheet.Cell(1, 10).Value = "ID Card Number";
            worksheet.Cell(1, 11).Value = "Bank Name";
            worksheet.Cell(1, 12).Value = "Bank Account Number";
            worksheet.Cell(1, 13).Value = "Bank IFSC Code";
            worksheet.Cell(1, 14).Value = "Bank Branch Details";

            // Style headers
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Add data
            for (int i = 0; i < applicants.Count; i++)
            {
                var applicant = applicants[i];
                var row = i + 2;

                worksheet.Cell(row, 1).Value = applicant.FullName;

                if (applicant.Dependents != null && applicant.Dependents.Count > 0)
                {
                    var obj = applicant.Dependents.FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(obj.LastName))
                    {
                        worksheet.Cell(row, 2).Value = obj.FirstName;
                    }
                    else
                        worksheet.Cell(row, 2).Value = $"{obj.FirstName} {obj.LastName} ";
                }
                else
                {
                    worksheet.Cell(row, 2).Value = "";
                }
                worksheet.Cell(row, 3).Value = applicant.DateOfBirth;
                worksheet.Cell(row, 4).Value = applicant.Gender.ToString();
                worksheet.Cell(row, 5).Value = applicant.MobileNumber;
                worksheet.Cell(row, 6).Value = applicant.Salary;
                worksheet.Cell(row, 7).Value = applicant.AssociatedOrganization;
                worksheet.Cell(row, 8).Value = applicant.Address;
                worksheet.Cell(row, 9).Value = applicant.IdCardType.ToString();
                worksheet.Cell(row, 10).Value = applicant.IdCardNumber;

                if (applicant.BankDetails != null)
                {
                    worksheet.Cell(row, 11).Value = applicant.BankDetails.BankName;
                    worksheet.Cell(row, 12).Value = applicant.BankDetails.BankAccountNumber;
                    worksheet.Cell(row, 13).Value = applicant.BankDetails.BankIfscCode;
                    worksheet.Cell(row, 14).Value = applicant.BankDetails.BankBranchDetails;
                }
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Applicants_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private async Task<(byte[] fileContent, string contentType, string fileName)> ExportToCsvAsync(List<ApplicantDto> applicants)
        {
            var records = applicants.Select(a => new
            {
                a.FullName,
                DateOfBirth = a.DateOfBirth.ToString("yyyy-MM-dd"),
                a.Gender,
                a.MobileNumber,
                a.Salary,
                a.AssociatedOrganization,
                a.Address,
                a.IdCardType,
                a.IdCardNumber,
                BankName = a.BankDetails?.BankName,
                BankAccountNumber = a.BankDetails?.BankAccountNumber,
                BankIfscCode = a.BankDetails?.BankIfscCode,
                BankBranchDetails = a.BankDetails?.BankBranchDetails
            });

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            await csv.WriteRecordsAsync(records);
            await writer.FlushAsync();

            return (memoryStream.ToArray(), "text/csv", $"Applicants_{DateTime.Now:yyyyMMdd}.csv");
        }

        //private async Task<(byte[] fileContent, string contentType, string fileName)> ExportToPdfAsync(List<ApplicantDto> applicants)
        //{
        //    using var memoryStream = new MemoryStream();
        //    var writer = new PdfWriter(memoryStream);
        //    var pdf = new PdfDocument(writer);
        //    var document = new Document(pdf);

        //    document.Add(new Paragraph("Insurance Policy Applicants").SetFontSize(16).SetBold());
        //    document.Add(new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}").SetFontSize(10));
        //    document.Add(new Paragraph("\n"));

        //    // Create a table for the applicants
        //    var table = new Table(6);
        //    table.SetWidth(UnitValue.CreatePercentValue(100));

        //    // Add headers
        //    table.AddHeaderCell("Full Name");
        //    table.AddHeaderCell("Date of Birth");
        //    table.AddHeaderCell("Gender");
        //    table.AddHeaderCell("Organization");
        //    table.AddHeaderCell("ID Card Number");
        //    table.AddHeaderCell("Bank Account");

        //    // Add data
        //    foreach (var applicant in applicants)
        //    {
        //        table.AddCell(applicant.FullName);
        //        table.AddCell(applicant.DateOfBirth.ToString("yyyy-MM-dd"));
        //        table.AddCell(applicant.Gender);
        //        table.AddCell(applicant.AssociatedOrganization);
        //        table.AddCell($"{applicant.IdCardType}: {applicant.IdCardNumber}");
        //        table.AddCell(applicant.BankDetails?.BankAccountNumber ?? "N/A");
        //    }

        //    document.Add(table);
        //    document.Close();

        //    return (memoryStream.ToArray(), "application/pdf", $"Applicants_{DateTime.Now:yyyyMMdd}.pdf");
        //}

    }



}
