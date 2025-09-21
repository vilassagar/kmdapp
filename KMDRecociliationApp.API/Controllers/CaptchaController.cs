using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using KMDRecociliationApp.Domain.Auth;
using KMDRecociliationApp.Domain.ConfigurationModels;



namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [System.Web.Http.AllowAnonymous]
    public class CaptchaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
      
        private readonly CaptchaConfiguration _captchaConfiguration;
        public CaptchaController(IHttpClientFactory httpClientFactory,
           Microsoft.Extensions.Options.IOptions<CaptchaConfiguration> captchaConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _captchaConfiguration = captchaConfiguration.Value;
        }

        [HttpPost]
        [Route("verifycaptcha")]
        public async Task<IActionResult> Post([FromBody] CaptchaRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Captcha is required");
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={_captchaConfiguration.SecretKey}&response={request.Token}",
                null
            );

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(jsonResponse);

            if (captchaResponse != null && captchaResponse.Success)
            {
                return Ok(new { isVerified = true });
            }
            else
            {
                return Ok(new { isVerified = false });
            }
        }
    }


}

