using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Wrapper;
using System.Text.Json;

namespace BusTicketSystem.Web.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteErrorResponseAsync(context, 401, ex.Message);
            }
            catch (Exception)
            {
                await WriteErrorResponseAsync(context, 500, "Something went wrong on the server.");
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.ErrorResponse(message, statusCode);

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}