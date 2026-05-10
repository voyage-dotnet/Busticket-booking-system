using BusTicketSystem.Web.Exceptions;
using System.Net;
using System.Text.Json;

namespace BusTicketSystem.Web.Middlewares
{
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string message;

            switch (ex)
            {
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;          // 400
                    message = ex.Message;
                    break;
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;            // 404
                    message = ex.Message;
                    break;
                case ForbiddenException:
                    statusCode = (int)HttpStatusCode.Forbidden;           // 403
                    message = ex.Message;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError; // 500
                    message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            var response = ApiResponse<object>.FailureResponse(
                message,
                new List<string> { ex.Message },
                statusCode
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }
    }
}
