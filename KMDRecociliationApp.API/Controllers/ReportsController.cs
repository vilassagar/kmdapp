using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using KMDRecociliationApp.Domain.ReportParamModels;
using KMDRecociliationApp.API.Common.Filters;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ReportsController : ControllerBase
    {
        //private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly ReportsRepository _reportsRepository;
        private readonly ApplicationDbContext _context;
        private readonly string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public ReportsController(ReportsRepository reportsRepository,
           ApplicationDbContext context, ILogger<UserController> logger
            //, IValidator<DTOAssociation> validator
            ,RoleRepository roleRepository)
        {
            // _tokenService = tokenService;
            _logger = logger;
            _reportsRepository = reportsRepository;
            _context = context;
            //_validator = validator;
            //_kMDAPISecretKey = kmdapikey.Value;
            //_roleRepository = roleRepository;
        }

        [HttpGet("GetDailyCountAssociationWise")]
        [IpAuthorizationFilter]
        //public  IActionResult GetDailyCountAssociationWise([FromQuery] DateTime reportDate)
        //{

        //    //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    var fileName = $"ReportUsersByCreationDate_{DateTime.Now:yyyyMMdd}.xlsx";
        //    var table = _reportsRepository.GetDailyCountAssociationWise(reportDate);
        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Sheet1");
        //        worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content,
        //                        contentType,
        //                        fileName);
        //        }
        //    }



        //}
        ////{
        //    // Define the parameters for the stored procedure
        //    var dateParam = new SqlParameter("@ReportDate", reportDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.DailyCountAssociationWiseReportDataModels
        //        .FromSqlRaw("EXEC GetUsersByCreationDate @ReportDate", dateParam)
        //        .ToListAsync();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Country Code";
        //        worksheet.Cell(1, 5).Value = "Mobile Number";
        //        worksheet.Cell(1, 6).Value = "Date Of Birth";
        //        worksheet.Cell(1, 7).Value = "User Type";
        //        worksheet.Cell(1, 8).Value = "Gender";
        //        worksheet.Cell(1, 9).Value = "Organisation Name";
        //        worksheet.Cell(1, 10).Value = "Association Name";
        //        worksheet.Cell(1, 11).Value = "EMP ID / PFNo";
        //        worksheet.Cell(1, 12).Value = "Address";
        //        worksheet.Cell(1, 13).Value = "City";
        //        worksheet.Cell(1, 14).Value = "State";
        //        worksheet.Cell(1, 15).Value = "Country";
        //        worksheet.Cell(1, 16).Value = "Pincode";
        //        worksheet.Cell(1, 17).Value = "Profile Complete";
        //        worksheet.Cell(1, 18).Value = "CreatedAt";

        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = data[i].CountryCode ?? "";
        //            worksheet.Cell(i + 2, 5).Value = data[i].MobileNumber ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].DOB.ToString("dd-MM-yyyy") ?? "";
        //            UserType userTypeEnum = (UserType)(data[i].UserType ?? 0); 
        //            worksheet.Cell(i + 2, 7).Value = userTypeEnum.ToString();
        //            Gender genderEnum = (Gender)(data[i].Gender ?? 0);
        //            worksheet.Cell(i + 2, 8).Value = genderEnum.ToString();
        //            worksheet.Cell(i + 2, 9).Value = data[i].Organisation ?? ""; // Assuming it's OrganisationName, not Organisation
        //            worksheet.Cell(i + 2, 10).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 11).Value = data[i].EMPIDPFNo ?? "";
        //            worksheet.Cell(i + 2, 12).Value = data[i].Address ?? "";
        //            worksheet.Cell(i + 2, 13).Value = data[i].City ?? "";
        //            worksheet.Cell(i + 2, 14).Value = data[i].State ?? ""; // Assuming StateName, not State
        //            worksheet.Cell(i + 2, 15).Value = data[i].Country ?? ""; // Assuming CountryName, not Country
        //            worksheet.Cell(i + 2, 16).Value = data[i].Pincode ?? "";
        //            worksheet.Cell(i + 2, 17).Value = data[i].IsProfileComplete ? "Yes" : "No";
        //            worksheet.Cell(i + 2, 18).Value = data[i].CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //      //  var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportUsersByCreationDate_{reportDate:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return File(stream, contentType, fileName);



        //    }
        //}

        [HttpPost("GetAssociationWisePaymentDetails")]
        //[IpAuthorizationFilter]
        public IActionResult GetAssociationWisePaymentDetails(AssociationWisePaymentDetailsReportParamModel associationWisePaymentDetailsReportParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportAssociationWisePaymentDetails_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetAssociationWisePaymentDetails(associationWisePaymentDetailsReportParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }

        [HttpGet("GetExtractPensionerPaymentDetails")]
        [IpAuthorizationFilter]
        public IActionResult GetExtractPensionerPaymentDetails(
            [FromQuery] int paymentTypeId, [FromQuery] int paymentStatusId
            , [FromQuery] int campaignId, [FromQuery] int associationId
            )
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"PensionerPaymentDetails_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetExtractPensionerPaymentDetails(paymentTypeId, paymentStatusId, campaignId,associationId);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }

        [HttpGet("GetAssociationExtract")]
        [IpAuthorizationFilter]
        public IActionResult GetAssociationExtract(int id
          // [FromQuery] DateTime startDate, [FromQuery] DateTime EndDate
          )
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"AssociationExtract{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetAssociationExtract(id);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }

        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", associationWisePaymentDetailsReportParamModel.AssociationId);
        //    var paymentStatusIdParam = new SqlParameter("@PaymentStatusId", associationWisePaymentDetailsReportParamModel.PaymentStatusId);
        //    var startDateParam = new SqlParameter("@StartDate", associationWisePaymentDetailsReportParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", associationWisePaymentDetailsReportParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.AssociationWisePaymentDetailsDataModels
        //        .FromSqlRaw("EXEC GetAssociationWisePaymentDetails @AssociationID, @PaymentStatusId, @StartDate, @EndDate",
        //                    associationIdParam, paymentStatusIdParam, startDateParam, endDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Organisation Name";
        //        worksheet.Cell(1, 6).Value = "Association Name";
        //        worksheet.Cell(1, 7).Value = "Payment Date";
        //        worksheet.Cell(1, 8).Value = "Premimum Amount";
        //        worksheet.Cell(1, 9).Value = "Paid Amount";
        //        worksheet.Cell(1, 10).Value = "Payment Mode";
        //        worksheet.Cell(1, 11).Value = "Payment Type";
        //        worksheet.Cell(1, 12).Value = "Payment Status";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 7).Value = data[i].PaymentDate.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 8).Value = data[i].PremimumAmount ?? 0;
        //            worksheet.Cell(i + 2, 9).Value = data[i].PaidAmount ?? 0;
        //            //worksheet.Cell(i + 2, 10).Value = data[i].PaymentType ?? 0;
        //            PaymentMode paymentModeEnum = (PaymentMode)(data[i].PaymentMode ?? 0);
        //            worksheet.Cell(i + 2, 10).Value = paymentModeEnum.ToString();
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 11).Value = paymentTypeEnum.ToString();

        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].PaymentStatus ?? 0);
        //            worksheet.Cell(i + 2, 12).Value = paymentStatusEnum.ToString();


        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //        //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportAssociationWisePaymentDetails_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetCompletedForms")]
        [IpAuthorizationFilter]
        public IActionResult GetCompletedForms(CompletedFormsParamModel completedFormsParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportCompletedForms_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetCompletedForms(completedFormsParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", completedFormsParamModel.AssociationId);
        //    var reportDateParam = new SqlParameter("@ReportDate", completedFormsParamModel.ReportDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.CompletedFormsDataModels
        //        .FromSqlRaw("EXEC GetCompletedForms @AssociationID, @ReportDate",
        //                    associationIdParam, reportDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        //worksheet.Cell(1, 1).Value = "CountryCode";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Date Of Birth";
        //        worksheet.Cell(1, 6).Value = "UserType";
        //        worksheet.Cell(1, 7).Value = "Gender";
        //        worksheet.Cell(1, 8).Value = "Organisation Name";
        //        worksheet.Cell(1, 9).Value = "Association Name";
        //        worksheet.Cell(1, 10).Value = "EMP ID / PFNo";
        //        worksheet.Cell(1, 11).Value = "Address";
        //        worksheet.Cell(1, 12).Value = "City";
        //        worksheet.Cell(1, 13).Value = "State";
        //        worksheet.Cell(1, 14).Value = "Country";
        //        worksheet.Cell(1, 15).Value = "Pincode";
        //        worksheet.Cell(1, 16).Value = "Profile Complete";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].DOB.ToString("dd-MM-yyyy") ?? "";
        //            UserType userTypeEnum = (UserType)(data[i].UserType ?? 0);
        //            worksheet.Cell(i + 2, 6).Value = userTypeEnum.ToString();
        //            Gender genderEnum = (Gender)(data[i].Gender ?? 0);
        //            worksheet.Cell(i + 2, 7).Value = genderEnum.ToString();
        //            worksheet.Cell(i + 2, 8).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 9).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 10).Value = data[i].EMPIDPFNo ?? "";
        //            worksheet.Cell(i + 2, 11).Value = data[i].Address ?? "";
        //            worksheet.Cell(i + 2, 12).Value = data[i].City ?? "";
        //            worksheet.Cell(i + 2, 13).Value = data[i].State ?? "";
        //            worksheet.Cell(i + 2, 14).Value = data[i].Country ?? "";
        //            worksheet.Cell(i + 2, 15).Value = data[i].Pincode ?? "";
        //            ProfileComplete profileCompleteEnum = (ProfileComplete)(data[i].IsProfileComplete==true ?1:0);
        //            worksheet.Cell(i + 2, 16).Value = profileCompleteEnum.ToString();                  


        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //       // var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportCompletedForms_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetIncompleteTransactions")]
        [IpAuthorizationFilter]
        public IActionResult GetIncompleteTransactions(IncompleteTransactionParamModel incompleteTransactionParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportIncompleteTransaction_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetIncompleteTransactions(incompleteTransactionParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", incompleteTransactionParamModel.AssociationId);
        //    var reportDateParam = new SqlParameter("@ReportDate", incompleteTransactionParamModel.ReportDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.IncompleteTransactionDataModels
        //        .FromSqlRaw("EXEC GetIncompleteTransaction @AssociationID, @ReportDate",
        //                    associationIdParam, reportDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Organisation Name";
        //        worksheet.Cell(1, 6).Value = "Association Name";
        //        worksheet.Cell(1, 7).Value = "Payment Date";
        //        worksheet.Cell(1, 8).Value = "Premimum Amount";
        //        worksheet.Cell(1, 9).Value = "Paid Amount";
        //        worksheet.Cell(1, 10).Value = "Payment Mode";
        //        worksheet.Cell(1, 11).Value = "Payment Type";
        //        worksheet.Cell(1, 12).Value = "Payment Status";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 7).Value = data[i].PaymentDate.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 8).Value = data[i].PremimumAmount ?? 0;
        //            worksheet.Cell(i + 2, 9).Value = data[i].PaidAmount ?? 0;
        //            //worksheet.Cell(i + 2, 10).Value = data[i].PaymentType ?? 0;
        //            PaymentMode paymentModeEnum = (PaymentMode)(data[i].PaymentMode ?? 0);
        //            worksheet.Cell(i + 2, 10).Value = paymentModeEnum.ToString();
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 11).Value = paymentTypeEnum.ToString();

        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].PaymentStatus ?? 0);
        //            worksheet.Cell(i + 2, 12).Value = paymentStatusEnum.ToString();

        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //        //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportIncompleteTransaction_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetBouncedPayments")]
        [IpAuthorizationFilter]
        public IActionResult GetBouncedPayments(GetBouncedPaymentsReportParamModel getBouncedPaymentsReportParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportBouncedPayments_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetBouncedPayments(getBouncedPaymentsReportParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", getBouncedPaymentsReportParamModel.AssociationId);
        //    var startDateParam = new SqlParameter("@StartDate", getBouncedPaymentsReportParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", getBouncedPaymentsReportParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.AssociationWisePaymentDetailsDataModels
        //        .FromSqlRaw("EXEC GetBouncedPayments @AssociationID, @StartDate, @EndDate",
        //                    associationIdParam, startDateParam, endDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Organisation Name";
        //        worksheet.Cell(1, 6).Value = "Association Name";
        //        worksheet.Cell(1, 7).Value = "Payment Date";
        //        worksheet.Cell(1, 8).Value = "Premimum Amount";
        //        worksheet.Cell(1, 9).Value = "Paid Amount";
        //        worksheet.Cell(1, 10).Value = "Payment Mode";
        //        worksheet.Cell(1, 11).Value = "Payment Type";
        //        worksheet.Cell(1, 12).Value = "Payment Status";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 7).Value = data[i].PaymentDate.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 8).Value = data[i].PremimumAmount ?? 0;
        //            worksheet.Cell(i + 2, 9).Value = data[i].PaidAmount ?? 0;
        //            //worksheet.Cell(i + 2, 10).Value = data[i].PaymentType ?? 0;
        //            PaymentMode paymentModeEnum = (PaymentMode)(data[i].PaymentMode ?? 0);
        //            worksheet.Cell(i + 2, 10).Value = paymentModeEnum.ToString();
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 11).Value = paymentTypeEnum.ToString();

        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].PaymentStatus ?? 0);
        //            worksheet.Cell(i + 2, 12).Value = paymentStatusEnum.ToString();


        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //        //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportBouncedPayments_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetReconcileedOnlinePayments")]
        [IpAuthorizationFilter]
        public IActionResult GetReconcileedOnlinePayments(GetReconciledOnlinePaymentsParamModel getReconciledOnlinePaymentsParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportReconciledOnlinePayments_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetReconcileedOnlinePayments(getReconciledOnlinePaymentsParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", getReconciledOnlinePaymentsParamModel.AssociationId);
        //    var startDateParam = new SqlParameter("@StartDate", getReconciledOnlinePaymentsParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", getReconciledOnlinePaymentsParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.AssociationWisePaymentDetailsDataModels
        //        .FromSqlRaw("EXEC GetReconcileedOnlinePayments @AssociationID, @StartDate, @EndDate",
        //                    associationIdParam, startDateParam, endDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Organisation Name";
        //        worksheet.Cell(1, 6).Value = "Association Name";
        //        worksheet.Cell(1, 7).Value = "Payment Date";
        //        worksheet.Cell(1, 8).Value = "Premimum Amount";
        //        worksheet.Cell(1, 9).Value = "Paid Amount";
        //        worksheet.Cell(1, 10).Value = "Payment Mode";
        //        worksheet.Cell(1, 11).Value = "Payment Type";
        //        worksheet.Cell(1, 12).Value = "Payment Status";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 7).Value = data[i].PaymentDate.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 8).Value = data[i].PremimumAmount ?? 0;
        //            worksheet.Cell(i + 2, 9).Value = data[i].PaidAmount ?? 0;
        //            //worksheet.Cell(i + 2, 10).Value = data[i].PaymentType ?? 0;
        //            PaymentMode paymentModeEnum = (PaymentMode)(data[i].PaymentMode ?? 0);
        //            worksheet.Cell(i + 2, 10).Value = paymentModeEnum.ToString();
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 11).Value = paymentTypeEnum.ToString();

        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].PaymentStatus ?? 0);
        //            worksheet.Cell(i + 2, 12).Value = paymentStatusEnum.ToString();


        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //        //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportReconciledOnlinePayments_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetOfflinePayments")]
        [IpAuthorizationFilter]
        public IActionResult GetOfflinePayments(GetOfflinePaymentsParamModel getOfflinePaymentsParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportOfflinePayments_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetOfflinePayments(getOfflinePaymentsParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", getOfflinePaymentsParamModel.AssociationId);
        //    var startDateParam = new SqlParameter("@StartDate", getOfflinePaymentsParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", getOfflinePaymentsParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.AssociationWisePaymentDetailsDataModels
        //        .FromSqlRaw("EXEC GetOfflinePayments @AssociationID, @StartDate, @EndDate",
        //                    associationIdParam, startDateParam, endDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Organisation Name";
        //        worksheet.Cell(1, 6).Value = "Association Name";
        //        worksheet.Cell(1, 7).Value = "Payment Date";
        //        worksheet.Cell(1, 8).Value = "Premimum Amount";
        //        worksheet.Cell(1, 9).Value = "Paid Amount";
        //        worksheet.Cell(1, 10).Value = "Payment Mode";
        //        worksheet.Cell(1, 11).Value = "Payment Type";
        //        worksheet.Cell(1, 12).Value = "Payment Status";


        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "";
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 6).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 7).Value = data[i].PaymentDate.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 8).Value = data[i].PremimumAmount ?? 0;
        //            worksheet.Cell(i + 2, 9).Value = data[i].PaidAmount ?? 0;
        //            //worksheet.Cell(i + 2, 10).Value = data[i].PaymentType ?? 0;
        //            PaymentMode paymentModeEnum = (PaymentMode)(data[i].PaymentMode ?? 0);
        //            worksheet.Cell(i + 2, 10).Value = paymentModeEnum.ToString();
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 11).Value = paymentTypeEnum.ToString();

        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].PaymentStatus ?? 0);
        //            worksheet.Cell(i + 2, 12).Value = paymentStatusEnum.ToString();


        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //       // var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportOfflinePayments_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}

        [HttpPost("GetCorrectionReport")]
        [IpAuthorizationFilter]
        public IActionResult GetCorrectionReport(GetCorrectionReportParamModel getCorrectionReportParamModel)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ReportCorrectionReport_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetCorrectionReport(getCorrectionReportParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", getCorrectionReportParamModel.AssociationId);
        //    var startDateParam = new SqlParameter("@StartDate", getCorrectionReportParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", getCorrectionReportParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.CorrectionReportDataModels
        //        .FromSqlRaw("EXEC GetCorrectionReport @AssociationID, @StartDate, @EndDate",
        //                    associationIdParam, startDateParam, endDateParam)
        //        .ToListAsync();

        [HttpPost("GetRefundReports")]
        [IpAuthorizationFilter]
        public IActionResult GetRefundReports(GetRefundReportsParamModel getRefundReportsParamModel)
        {
            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"GetRefundReports_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetRefundReports(getRefundReportsParamModel);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }
        }
        //{
        //    var associationIdParam = new SqlParameter("@AssociationID", getRefundReportsParamModel.AssociationId);
        //    var organisationIdParam = new SqlParameter("@OrganisationId", getRefundReportsParamModel.OrganisationId);
        //    var startDateParam = new SqlParameter("@StartDate", getRefundReportsParamModel.StartDate);
        //    var endDateParam = new SqlParameter("@EndDate", getRefundReportsParamModel.EndDate);

        //    // Execute the stored procedure and get the results
        //    var data = await _context.GetRefundReportsDataModels
        //        .FromSqlRaw("EXEC getRefundReports @AssociationID,@OrganisationId, @StartDate, @EndDate",
        //                    associationIdParam, organisationIdParam, startDateParam, endDateParam)
        //        .ToListAsync();


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Report");

        //        // Add headers
        //        worksheet.Cell(1, 1).Value = "First Name";
        //        worksheet.Cell(1, 2).Value = "Last Name";
        //        worksheet.Cell(1, 3).Value = "Email";
        //        //worksheet.Cell(1, 4).Value = "Country Code";
        //        worksheet.Cell(1, 4).Value = "Mobile Number";
        //        worksheet.Cell(1, 5).Value = "Date Of Birth";
        //        worksheet.Cell(1, 6).Value = "UserType";
        //        worksheet.Cell(1, 7).Value = "Gender";
        //        worksheet.Cell(1, 8).Value = "Organisation Name";
        //        worksheet.Cell(1, 9).Value = "Association Name";
        //        worksheet.Cell(1, 10).Value = "EMP ID / PFNo";
        //        worksheet.Cell(1, 11).Value = "Address";
        //        worksheet.Cell(1, 12).Value = "City";
        //        worksheet.Cell(1, 13).Value = "State";
        //        worksheet.Cell(1, 14).Value = "Country";
        //        worksheet.Cell(1, 15).Value = "Pincode";
        //        //PolicyId],
        //        //RetireeId],
        //        worksheet.Cell(1, 16).Value = "Refund Amount"; 
        //        worksheet.Cell(1, 17).Value = "Refund Request Date";
        //        worksheet.Cell(1, 18).Value = "Refund Status";
        //        worksheet.Cell(1, 19).Value = "Comment";
        //        worksheet.Cell(1, 20).Value = "Refund Requst Handled By";
        //        worksheet.Cell(1, 21).Value = "Refund Payment Type";

        //        //--Cheque = 1, NEFT = 2,UPI = 3, Gateway = 4
        //        //-- Conditional Payment Details for UPI
        //        worksheet.Cell(1, 22).Value = "UPI Transaction Number";
        //        worksheet.Cell(1, 23).Value = "UPI Amount";
        //        worksheet.Cell(1, 24).Value = "UPI Date";

        //        //--Conditional Payment Details for NEFT
        //        worksheet.Cell(1, 25).Value = "NEFT Transaction Id";
        //        worksheet.Cell(1, 26).Value = "NEFT Amount";
        //        worksheet.Cell(1, 27).Value = "NEFT Date";
        //        worksheet.Cell(1, 28).Value = "NEFT BankName";
        //        worksheet.Cell(1, 29).Value = "NEFT BranchName";
        //        worksheet.Cell(1, 30).Value = "NEFT AccountNumber";
        //        worksheet.Cell(1, 31).Value = "NEFT IfscCode";

        //        //--Conditional Payment Details for Cheque
        //        worksheet.Cell(1, 32).Value = "Cheque Number";
        //        worksheet.Cell(1, 33).Value = "Cheque Amount";
        //        worksheet.Cell(1, 34).Value = "Cheque Date";
        //        worksheet.Cell(1, 35).Value = "Cheque Bank Name";

        //        // Add data to cells
        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, 1).Value = data[i].FirstName ?? "";
        //            worksheet.Cell(i + 2, 2).Value = data[i].LastName ?? "";
        //            worksheet.Cell(i + 2, 3).Value = data[i].Email ?? "" ;
        //            worksheet.Cell(i + 2, 4).Value = (data[i].CountryCode ?? "") + (data[i].MobileNumber ?? "");
        //            worksheet.Cell(i + 2, 5).Value = data[i].DOB?.ToString("dd-MM-yyyy") ?? "";
        //            UserType userTypeEnum = (UserType)(data[i].UserType ?? 0);
        //            worksheet.Cell(i + 2, 6).Value = userTypeEnum.ToString();
        //            Gender genderEnum = (Gender)(data[i].Gender ?? 0);
        //            worksheet.Cell(i + 2, 7).Value = genderEnum.ToString();
        //            worksheet.Cell(i + 2, 8).Value = data[i].OrganisationName ?? "";
        //            worksheet.Cell(i + 2, 9).Value = data[i].AssociationName ?? "";
        //            worksheet.Cell(i + 2, 10).Value = data[i].EMPIDPFNo ?? "";
        //            worksheet.Cell(i + 2, 11).Value = data[i].Address ?? "";  
        //            worksheet.Cell(i + 2, 12).Value = data[i].City ?? "";
        //            worksheet.Cell(i + 2, 13).Value = data[i].State ?? "";
        //            worksheet.Cell(i + 2, 14).Value = data[i].Country ?? "";
        //            worksheet.Cell(i + 2, 15).Value = data[i].Pincode ?? "";
        //            worksheet.Cell(i + 2, 16).Value = data[i].RefundAmount ?? 0;
        //            worksheet.Cell(i + 2, 17).Value = data[i].RefundRequestDate?.ToString("dd-MM-yyyy") ?? "";
        //            PaymentStatus paymentStatusEnum = (PaymentStatus)(data[i].Status ?? 0);
        //            worksheet.Cell(i + 2, 18).Value = paymentStatusEnum.ToString();
        //            worksheet.Cell(i + 2, 19).Value = data[i].Comment ?? "";
        //            worksheet.Cell(i + 2, 20).Value = data[i].RefundRequstHandledBy ?? 0;
        //            PaymentTypes paymentTypeEnum = (PaymentTypes)(data[i].PaymentType ?? 0);
        //            worksheet.Cell(i + 2, 21).Value = paymentTypeEnum.ToString();
        //            worksheet.Cell(i + 2, 22).Value = data[i].TransactionNumber ?? "";
        //            worksheet.Cell(i + 2, 23).Value = data[i].UPIAmount ?? 0;
        //            worksheet.Cell(i + 2, 24).Value = data[i].UPIDate?.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 25).Value = data[i].NEFTTransactionId ?? "";
        //            worksheet.Cell(i + 2, 26).Value = data[i].NEFTAmount ?? 0;
        //            worksheet.Cell(i + 2, 27).Value = data[i].NEFTDate?.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 28).Value = data[i].NEFTBankName ?? "";
        //            worksheet.Cell(i + 2, 29).Value = data[i].NEFTBranchName ?? "";
        //            worksheet.Cell(i + 2, 30).Value = data[i].NEFTAccountNumber ?? "";
        //            worksheet.Cell(i + 2, 31).Value = data[i].NEFTIfscCode ?? "";
        //            worksheet.Cell(i + 2, 32).Value = data[i].ChequeNumber ?? "";
        //            worksheet.Cell(i + 2, 33).Value = data[i].ChequeAmount ?? 0;
        //            worksheet.Cell(i + 2, 34).Value = data[i].ChequeDate?.ToString("dd-MM-yyyy") ?? "";
        //            worksheet.Cell(i + 2, 35).Value = data[i].ChequeBankName ?? "";
                    
                    
        //        }

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;

        //       // var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = $"ReportRefundReports_{DateTime.Now:yyyyMMdd}.xlsx";

        //        // Return the stream directly as a FileStreamResult
        //        return new FileStreamResult(stream, contentType)
        //        {
        //            FileDownloadName = fileName
        //        };
        //    }
        //}
       
        [HttpPost("GetInsuranceCompanyReport")]
        [IpAuthorizationFilter]
        public IActionResult GetInsuranceCompanyReport(InsuranceCompanyReportModel insuranceCompanyReport)
        {

            //var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"InsuranceCompanyReport_{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetInsuranceCompanyReport(insuranceCompanyReport);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }
            

            
        }


    }


}

