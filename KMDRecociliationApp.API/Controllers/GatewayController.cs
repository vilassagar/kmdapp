using Microsoft.AspNetCore.Mvc;
using Serilog;
using RestSharp;
using Newtonsoft.Json;
using KMDRecociliationApp.Domain.PaymentDTO;
using Microsoft.Extensions.Options;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Data;
using Microsoft.EntityFrameworkCore;

namespace KMDRecociliationApp.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class GatewayController : ApiBaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly GoDigitPaymentGateway _goDigitPaymentGateway;
        private readonly ApplicationDbContext context;
        private readonly PaymentRepository _paymentRepository;

        public GatewayController(IHttpClientFactory httpClientFactory
            , IConfiguration configuration
            , IOptions<GoDigitPaymentGateway> goDigitPaymentGateway
            , PaymentRepository paymentRepository
             , ApplicationDbContext _context) : base(_context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _goDigitPaymentGateway = goDigitPaymentGateway.Value;
            context = _context;
            _paymentRepository = paymentRepository;
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(DTOPaymentPolicy policy)
        {


            if(policy.PolicyId<=0 || policy.PaymentRequestBody.requestReference=="0")
                return BadRequest();

            Log.Information("Process Payment started");
            _paymentRepository.CurrentUser = HttpContext.User;
            string token = "";
            RestClient _authClient;
            var optionsauth = new RestClientOptions(_goDigitPaymentGateway.AuthEndPoint.url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = 90000
            };
            _authClient = new RestClient(optionsauth);

            try
            {
                Log.Information("auth api call started");
                var authrequest = new RestRequest("auth", Method.Post);

                // Add headers
                authrequest.AddHeader("Content-Type", "application/json");
                authrequest.AddHeader("Cookie", _goDigitPaymentGateway.AuthEndPoint.Cookie);

                // Add request body
                var authRequest = new PaymentAuthenticationRequest
                {
                    Username = _goDigitPaymentGateway.AuthEndPoint.Username,
                    Password = _goDigitPaymentGateway.AuthEndPoint.Password
                };
                authrequest.AddJsonBody(authRequest);
                var authresponse = await _authClient.ExecuteAsync(authrequest);


                if (authresponse.IsSuccessStatusCode)
                {

                    var tokenResponse = JsonConvert.DeserializeObject<PaymentTokenResponse>(authresponse.Content);
                    token = tokenResponse.AccessToken;
                    Log.Information("Auth Request Success");
                    Log.Information($"token:  {token}");

                }
                else
                {
                    return StatusCode((int)authresponse.StatusCode);
                }
                Log.Information("auth api call end");
            }
            catch (Exception ex)
            {
                AppLogs appLogs = new AppLogs();
                appLogs.Auditdate = DateTime.Now;
                appLogs.UserId = _paymentRepository.UserId;
                appLogs.Recordtype = ApplogType.PaymentGateWay;
                appLogs.Comment = $"Payment GateWay authRequest request Error in Authenticate {ex.Message}";
                context.AppLogs.Add(appLogs);
                await context.SaveChangesAsync();

                Log.Fatal($"Error in Authenticate {ex.Message}");
                return StatusCode(500, $" Error in Authenticate : {ex.Message}");
            }
            Log.Information("Executor api call started");
            RestClient _client;
            var options = new RestClientOptions(_goDigitPaymentGateway.ExecutorEndPoint.url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = 90000
            };
            _client = new RestClient(options);
            var request = new RestRequest("executor", Method.Post);
            try
            {


                // Add headers
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("integrationId", _goDigitPaymentGateway.ExecutorEndPoint.IntegrationId);
                request.AddHeader("cookie", _goDigitPaymentGateway.ExecutorEndPoint.Cookie);
                request.AddHeader("Authorization", $"Bearer {token}");
                // Add request body

                RootObject root = new RootObject();
                root.mobileNumber = policy.PaymentRequestBody.MobileNumber;
                root.requestReference = policy.PaymentRequestBody.requestReference;
                root.successReturnUrl = policy.PaymentRequestBody.successReturnUrl;
                root.cancelReturnUrl = policy.PaymentRequestBody.cancelReturnUrl;
                root.email = policy.PaymentRequestBody.Email;
                root.floatRequests = new List<FloatRequest>();
                FloatRequest floatRequest = new FloatRequest();
                if (policy.PaymentRequestBody != null && policy.PaymentRequestBody.floatRequests != null &&
                    policy.PaymentRequestBody.floatRequests.Count > 0)
                {
                    // floatRequest.customerCode = policy.PaymentRequestBody.floatRequests.FirstOrDefault().customerCode;
                    floatRequest.premiumAmount = policy.PaymentRequestBody.floatRequests.FirstOrDefault().premiumAmount;
                }
                root.floatRequests.Add(floatRequest);
                root.notification = new Notification();
                root.notification.requestId = Convert.ToString(policy.PaymentRequestBody.requestReference);
                root.notification.method = "post";
                root.notification.uri = _goDigitPaymentGateway.NotificationURL.url;

                Log.Information("Json Convert SerializeObject started");

                var json = JsonConvert.SerializeObject(root);
                request.AddJsonBody(root);

                Log.Information($"request Object :" +
                 $"{JsonConvert.SerializeObject(request)}");

                var response = await _client.ExecuteAsync(request);
                Log.Information($"Response: {response}");
                Log.Information($"ProcessPayment: status : {response}");
                if (response.IsSuccessStatusCode)
                {
                   
                    var myDeserializedClass = JsonConvert.DeserializeObject<RootResponse>(response.Content);

                    if (myDeserializedClass != null)
                    {
                        var policyHeader = await context.PolicyHeader
                         .FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(policy.PaymentRequestBody.requestReference));

                        if (policyHeader != null)
                        {
                            policyHeader.PaymentStatus = PaymentStatus.Pending;
                            policyHeader.UpdatedAt = DateTime.Now;
                            policyHeader.UpdatedBy = _paymentRepository.UserId;
                            policyHeader.Comment = "-";
                            policyHeader.IsManual = false;
                            policyHeader.TotalPremimum = policy.TotalPremium;
                            policyHeader.DigitPaymentId = myDeserializedClass.digitPaymentId;

                            await context.SaveChangesAsync();
                        }
                    }

                    Log.Information("executor api call end");

                    return Ok(myDeserializedClass);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }


            }
            catch (Exception ex)
            {
                AppLogs appLogs = new AppLogs();
                appLogs.Auditdate = DateTime.Now;
                appLogs.UserId = policy.UserId;
                appLogs.Recordtype = ApplogType.PaymentGateWay;
                appLogs.Comment = $"Payment GateWay executor  RequestBody {JsonConvert.SerializeObject(request)} " +
                    $"Error in ProcessPayment {ex.Message}";
                context.AppLogs.Add(appLogs);
                await context.SaveChangesAsync();

                Log.Fatal($"Error in ProcessPayment {ex.Message}");
                return StatusCode(500, $"ProcessPayment: Internal server error: {ex.Message}");
            }




        }

        [HttpPost("UpdatePayment")]
        public async Task<IActionResult> UpdatePaymentStatus(NotifyPaymentStatusPayload payload)
        {
            _paymentRepository.CurrentUser = HttpContext.User;
            try
            {
                if (payload.PolicyId == null || payload.PolicyId.Value == 0)
                {
                    return BadRequest();
                }

                AppLogs appLogs1 = new AppLogs();
                appLogs1.Auditdate = DateTime.Now;
                appLogs1.UserId = _paymentRepository.UserId;
                appLogs1.Recordtype = ApplogType.PaymentGateWay;
                appLogs1.Comment = $"Payment GateWay started transaction number update {payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
                context.AppLogs.Add(appLogs1);
                await context.SaveChangesAsync();

                // bool b = await _paymentRepository.UpdatePolicyOrder(payload, _paymentRepository.UserId, "UpdatePaymentStatus",false);
                AppLogs appLogs = new AppLogs();
                appLogs.Auditdate = DateTime.Now;
                appLogs.UserId = _paymentRepository.UserId;
                appLogs.Recordtype = ApplogType.PaymentGateWay;
                appLogs.Comment = $"Payment GateWay successful with transaction number {payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
                context.AppLogs.Add(appLogs);
                await context.SaveChangesAsync();
                return Ok();


            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in PaymentStatus {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("UpdateFailedPayment")]
        public async Task<IActionResult> UpdateFailedPaymentStatus(NotifyPaymentStatusPayload payload)
        {

            try
            {
                if (payload.PolicyId != null && payload.PolicyId.Value > 0)
                {
                    int policyid = payload.PolicyId.Value;
                    int userid = payload.UserId;
                    var policyheader = await context.PolicyHeader.AsNoTracking()
                                .Where(x => x.Id == policyid).FirstOrDefaultAsync();
                    if (policyheader != null)
                    {
                        //PolicyHeader policyHeader1 = new PolicyHeader();
                        policyheader.Id = policyheader.Id;
                        policyheader.PaymentStatus = PaymentStatus.Failed;
                        policyheader.UpdatedAt = DateTime.Now;
                        policyheader.UpdatedBy = policyheader.UserId;
                        context.PolicyHeader.Update(policyheader);
                        context.SaveChanges();
                    }

                    AppLogs appLogs = new AppLogs();
                    appLogs.Auditdate = DateTime.Now;
                    appLogs.Recordtype = ApplogType.PaymentGateWay;
                    appLogs.UserId = policyheader.UserId;
                    appLogs.Comment = $"Payment gateway failed: Policy Id:{policyheader.Id} Amount:{policyheader.TotalPremimum}";
                    context.AppLogs.Add(appLogs);
                    await context.SaveChangesAsync();

                    return Ok();
                }
                else
                    return BadRequest();


            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in UpdateFailedPayment {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
