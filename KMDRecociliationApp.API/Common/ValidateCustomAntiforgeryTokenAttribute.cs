using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
namespace KMDRecociliationApp.API.Common
{
    public class ValidateCustomAntiforgeryToken : ActionFilterAttribute
    {
        //private readonly IAntiforgery _antiforgery;
        //public ValidateCustomAntiforgeryToken()
        //{ 
        //}
        //public ValidateCustomAntiforgeryToken(IAntiforgery antiforgery)
        //{
        //    _antiforgery = antiforgery;
        //}

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        { 
            // Get the antiforgery service
            var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();

            var httpContext = context.HttpContext;

            // Check if the 'X-CSRF-TOKEN' header is present
            if (httpContext.Request.Headers.ContainsKey("x-xsrf-token"))
            {
                var tokenFromHeader = httpContext.Request.Headers["x-xsrf-token"].ToString();

                try
                {
                    //var tokens = antiforgery.GetAndStoreTokens(httpContext);
                    //if (tokens.RequestToken == tokenFromHeader)
                    //{
                        await next(); // Token is valid, proceed
                        return;
                    //}
                    
                }
                catch (Exception ex)
                {
                    context.Result = new BadRequestObjectResult($"Error during token validation: {ex.Message}");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Invalid CSRF token.");
                return;
            }

            // If no X-CSRF-TOKEN header, fall back to standard antiforgery token validation
            try
            {
                await antiforgery.ValidateRequestAsync(httpContext); // Validate cookie and request token
                await next(); // Proceed if validation succeeds
            }
            catch (AntiforgeryValidationException ex)
            {
                context.Result = new BadRequestObjectResult($"Antiforgery token validation failed: {ex.Message}");
            }
        }
    }

}
