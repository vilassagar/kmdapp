using Microsoft.AspNetCore.Mvc;

namespace KMDRecociliationApp.API.Common
{
    public class InternalServerErrorResult : IActionResult
    {
        private readonly object _value;

        public InternalServerErrorResult(object value)
        {
            _value = value;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_value)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
