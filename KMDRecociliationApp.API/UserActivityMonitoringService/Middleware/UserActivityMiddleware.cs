using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using UserActivityMonitoringService.Configuration;
using UserActivityMonitoringService.Model;
using UserActivityMonitoringService.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace UserActivityMonitoringService.Middleware
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserActivityMiddleware> _logger;
        private readonly UserActivityOptions _options;

        public UserActivityMiddleware(
            RequestDelegate next,
            ILogger<UserActivityMiddleware> logger,
            IOptions<UserActivityOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if logging is enabled
            if (!_options.IsEnabled)
            {
                await _next(context);
                return;
            }
            // Skip logging for excluded paths
            if (ShouldSkipLogging(context))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Request.Body;
            var requestBody = string.Empty;

            // Check if request contains file or multipart form data
            bool isFileUpload = IsFileUploadRequest(context.Request);

            try
            {
                if (_options.LogRequestBody && context.Request.ContentLength > 0 && !isFileUpload)
                {
                    using var streamReader = new StreamReader(context.Request.Body);
                    requestBody = await streamReader.ReadToEndAsync();

                    // Try to format as JSON if content type is application/json
                    if (IsJsonContent(context.Request))
                    {
                        try
                        {
                            var jsonElement = JsonSerializer.Deserialize<JsonElement>(requestBody);
                            requestBody = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
                            {
                                WriteIndented = true
                            });
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning(ex, "Failed to parse request body as JSON");
                        }
                    }

                    // Truncate request body if it exceeds maximum length
                    if (requestBody.Length > _options.MaxRequestBodyLength)
                    {
                        requestBody = requestBody.Substring(0, _options.MaxRequestBodyLength) + "...";
                    }

                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
                }

                // Get controller and action names using RouteData
                string controllerName = null;
                string actionName = null;

                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    var routeData = context.GetRouteData();
                    if (routeData?.Values != null)
                    {
                        if (routeData.Values.TryGetValue("controller", out var controller))
                        {
                            controllerName = controller?.ToString();
                        }
                        if (routeData.Values.TryGetValue("action", out var action))
                        {
                            actionName = action?.ToString();
                        }
                    }
                }

                var activity = new UserActivity
                {
                    UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous",
                    UserName = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "anonymous",
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    RequestPath = context.Request.Path,
                    QueryString = _options.LogQueryString ? context.Request.QueryString.ToString() : null,
                    Method = context.Request.Method,
                    RequestBody = requestBody,
                    Timestamp = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    ControllerName = controllerName ?? string.Empty,
                    ActionName = actionName ?? string.Empty
                };

                var originalResponseBody = context.Response.Body;
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                activity.StatusCode = context.Response.StatusCode;

                // Get scoped service from the request services
                var activityService = context.RequestServices.GetRequiredService<IUserActivityService>();
                await activityService.LogActivityAsync(activity);

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalResponseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserActivityMiddleware");
                throw;
            }
            finally
            {
                context.Request.Body = originalBodyStream;
            }
        }

        private bool IsJsonContent(HttpRequest request)
        {
            if (request.ContentType == null) return false;
            return request.ContentType.ToLower().Contains("application/json");
        }

        private bool ShouldSkipLogging(HttpContext context)
        {
            // Check if path is in excluded paths
            if (_options.ExcludePaths.Any(path =>
                context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Check if content type is in excluded content types
            var contentType = context.Request.ContentType;
            if (!string.IsNullOrEmpty(contentType) &&
                _options.ExcludeContentTypes.Any(excludedType =>
                    contentType.Contains(excludedType, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private bool IsFileUploadRequest(HttpRequest request)
        {
            // Check for multipart/form-data content type
            if (request.HasFormContentType)
            {
                return true;
            }

            // Check for common file upload content types
            var contentType = request.ContentType?.ToLower();
            return contentType != null && (
                contentType.Contains("multipart/form-data") ||
                contentType.Contains("application/octet-stream") ||
                contentType.StartsWith("image/") ||
                contentType.StartsWith("audio/") ||
                contentType.StartsWith("video/") ||
                contentType.Contains("application/pdf") ||
                contentType.Contains("application/msword") ||
                contentType.Contains("application/vnd.openxmlformats-officedocument") ||
                contentType.Contains("application/vnd.ms-excel") ||
                contentType.Contains("application/zip")
            );
        }
    }
}