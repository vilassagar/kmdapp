using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsrfController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public CsrfController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }
        [HttpGet("token")]
        public IActionResult GetCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken
                , new CookieOptions
            {
                HttpOnly = false,
                Secure = false,    // Use HTTPS
                SameSite = SameSiteMode.None, // Allows cross-site requests
                Path = "/"
                });
            var token= tokens.RequestToken;
            var ctoken = tokens.CookieToken;

            return Ok(new { Token = tokens.RequestToken });
        }
    }
}
