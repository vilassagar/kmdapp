using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Serilog;
using RestSharp;
using Newtonsoft.Json;
using KMDRecociliationApp.Domain.PaymentDTO;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.API.Common;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.InkML;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Data;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Azure;
using KMDRecociliationApp.Services;
using SkiaSharp;
using KMDRecociliationApp.Domain.Auth;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PaymentController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly GoDigitPaymentGateway _goDigitPaymentGateway;
        private readonly ApplicationDbContext context;
        private readonly PaymentRepository _paymentRepository;
        public PaymentController(IHttpClientFactory httpClientFactory
    , IConfiguration configuration
    , IOptions<GoDigitPaymentGateway> goDigitPaymentGateway
    , PaymentRepository paymentRepository
     , ApplicationDbContext _context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _goDigitPaymentGateway = goDigitPaymentGateway.Value;
            context = _context;
            _paymentRepository = paymentRepository;
        }



        [HttpPost("NotifyPaymentStatus")]
        public async Task<IActionResult> NotifyPaymentStatus(PaymentstatusPayload payload1)
        {
            Log.Information($"NotifyPaymentStatus Payment GateWay " +
                $"started transaction number update : {payload1.transactionNumber}" +
                $" premiumAmount: {payload1.premiumAmount} Policy Id: {payload1.requestId}");
            if (payload1.requestId == null || payload1.requestId == "0")
            {
                return BadRequest();
            }
            NotifyPaymentStatusPayload payload = new NotifyPaymentStatusPayload();
            payload.premiumAmount =Convert.ToDecimal(payload1.premiumAmount);
            payload.PolicyId =Convert.ToInt32(payload1.requestId);
            payload.transactionNumber = payload1.transactionNumber;
            var policyheader = await context.PolicyHeader.AsNoTracking()
                                           .Where(x => x.Id == payload.PolicyId).FirstOrDefaultAsync();
            if (policyheader != null)
            {
                payload.UserId = policyheader.UserId;
            }
            else
                return NotFound("Request Id not found");

            AppLogs appLogs1 = new AppLogs();
            appLogs1.Auditdate = DateTime.Now;
            appLogs1.UserId =0;
            appLogs1.Recordtype = ApplogType.PaymentGateWay;
            appLogs1.Comment = $"NotifyPaymentStatus Payment GateWay started transaction number update {payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
            context.AppLogs.Add(appLogs1);
            await context.SaveChangesAsync();

            bool b = await _paymentRepository.UpdatePolicyOrder(payload, 0, "NotifyPaymentStatus",false);
            Log.Information($"Policy Id: {payload1.requestId} Transaction number updated success {payload.transactionNumber}");
           AppLogs appLogs = new AppLogs();
            appLogs.Auditdate = DateTime.Now;
            appLogs.UserId = 0;
            appLogs.Recordtype = ApplogType.PaymentGateWay;
            appLogs.Comment = $"NotifyPaymentStatus Payment GateWay successful with transaction number {payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
            context.AppLogs.Add(appLogs);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
