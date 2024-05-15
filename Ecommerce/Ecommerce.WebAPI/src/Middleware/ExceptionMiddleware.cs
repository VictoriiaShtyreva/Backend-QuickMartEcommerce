using System.Net;
using System.Text.Json;
using Ecommerce.Core.src.Common;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                await HandleExceptionAsync(httpContext, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionMiddleware> logger)
        {
            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case AppException appEx:
                    // Handle custom application exceptions
                    context.Response.StatusCode = (int)appEx.StatusCode;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = appEx.Message
                    }));
                    logger.LogError($"Application error: {appEx.Message}", appEx);
                    break;
                case DbUpdateException dbUpdateEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    string innerMessage = dbUpdateEx.InnerException?.Message ?? "A database update error occurred.";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = innerMessage
                    }));
                    logger.LogError($"Database update error: {innerMessage}", dbUpdateEx);
                    break;
                case ArgumentException argEx:
                    // Bad Request for argument-related issues
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = argEx.Message
                    }));
                    logger.LogWarning($"Bad request: {argEx.Message}", argEx);
                    break;
                case ApplicationException appEx:
                    // Handle general application exceptions
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = appEx.Message
                    }));
                    logger.LogWarning($"Application error: {appEx.Message}", appEx);
                    break;
                case InvalidOperationException opEx:
                    // Unprocessable Entity for invalid operations
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = opEx.Message
                    }));
                    logger.LogWarning($"Invalid operation: {opEx.Message}", opEx);
                    break;
                default:
                    // Internal Server Error for general unhandled exceptions
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "An unexpected error occurred."
                    }));
                    logger.LogError($"Unhandled exception: {exception.Message}", exception);
                    break;
            }
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }

}