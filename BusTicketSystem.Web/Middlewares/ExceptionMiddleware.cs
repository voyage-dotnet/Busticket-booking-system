using BusTicketSystem.Web.ApiResponse;
using BusTicketSystem.Web.Exceptions;
using System.Net;
using System.Text.Json;

namespace BusTicketSystem.Web.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == 400
                && !context.Response.HasStarted)
            {
                await HandleValidationErrorAsync(context);
            }
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError,
                "Something went wrong. Please try again later.");
        }
    }

    private static async Task HandleValidationErrorAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse<string>.FailureResponse(
            "Validation failed. Please check your input.", statusCode: 400);
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;
        var response = ApiResponse<string>.FailureResponse(message, statusCode: (int)statusCode);
        var json     = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}