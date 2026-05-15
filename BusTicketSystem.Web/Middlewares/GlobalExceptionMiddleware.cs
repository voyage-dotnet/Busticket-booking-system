using BusTicketSystem.Web.ResponseWrapper;
using System;
using System.ComponentModel.Design.Serialization;
using System.Text.Json;
using Azure;
using BusTicketSystem.Web.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace BusTicketSystem.Web.Middlewares;

public class GlobalExceptionMiddleware
{
    public readonly RequestDelegate _next;
    public readonly ILogger<GlobalExceptionMiddleware> _logger;
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
            _logger.LogError(ex, "unhadled exception error");
            await HandleExceptionAsync(context, ex);
        }
    }

    public static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;

        switch (ex)
        {
            case BaseException base_ex:
                response = ApiResponse<object>.FailureResponse(base_ex.Message, base_ex.Errors, base_ex.StatusCode);
                break;
        default:
            var errorList = new List<string> { ex.Message };
            if (ex.InnerException != null)
            {
                errorList.Add($"Inner Exception: {ex.InnerException.Message}");
                if (ex.InnerException.InnerException != null)
                {
                    errorList.Add($"Deep Inner Exception: {ex.InnerException.InnerException.Message}");
                }
            }
            response = ApiResponse<object>.FailureResponse("Internal Server Error occurred", errorList, 500);
            break;
        }

        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}
