using Microsoft.AspNetCore.Mvc;

namespace KMDRecociliationApp.API.Common
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult InternalServerError(this ControllerBase controller, object value)
        {
            return new InternalServerErrorResult(value);
        }
    }
}
