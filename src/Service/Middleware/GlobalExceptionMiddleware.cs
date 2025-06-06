using Core.CommonModels.BaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Application.Middleware
{
    /// <summary>
    /// Global exception handling middleware for consistent error responses
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. RequestPath: {RequestPath}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new Acknowledgement();
            
            switch (exception)
            {
                case ArgumentNullException argEx:
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessageList.Add($"Missing required parameter: {argEx.ParamName}");
                    break;
                    
                case ArgumentException argEx:
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessageList.Add($"Invalid argument: {argEx.Message}");
                    break;
                    
                case UnauthorizedAccessException:
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.ErrorMessageList.Add("Unauthorized access");
                    break;
                    
                case KeyNotFoundException:
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessageList.Add("Resource not found");
                    break;
                    
                case TimeoutException:
                    response.StatusCode = HttpStatusCode.RequestTimeout;
                    response.ErrorMessageList.Add("Request timeout");
                    break;
                    
                default:
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.ErrorMessageList.Add("An internal server error occurred");
                    break;
            }

            context.Response.StatusCode = (int)response.StatusCode;
            
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
