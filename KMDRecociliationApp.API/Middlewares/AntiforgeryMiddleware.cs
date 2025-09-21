using Microsoft.AspNetCore.Antiforgery;

namespace KMDRecociliationApp.API.Middlewares
{
    public class AntiforgeryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api")) // Apply only for APIs
            {
                var tokens = _antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
                {
                    HttpOnly = false, // Make it accessible to JavaScript
                    Secure = true,    // Ensure it's secure (especially in production)
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });
            }

            await _next(context);
        }
    }

}
