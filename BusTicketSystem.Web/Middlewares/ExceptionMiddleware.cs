using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Wrapper;
using Microsoft.AspNetCore.Mvc;
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

            // Catch validation errors from FluentValidation (400 with ProblemDetails)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError,
                "Something went wrong. Please try again later.");
        }
    }

    private static async Task HandleValidationErrorAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var response = ApiResponse<string>.FailResponse(
            "Validation failed. Please check your input.");
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = ApiResponse<string>.FailResponse(message);
        var json     = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}