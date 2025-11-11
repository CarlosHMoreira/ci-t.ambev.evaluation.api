using FluentValidation;
using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, payload) = MapException(ex);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(payload, options);
        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, object Payload) MapException(Exception ex)
    {
        logger.LogError(ex, "Unhandled exception");
        return ex switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, BuildValidationApiResponse(ve)),
            UnauthorizedAccessException ua => (StatusCodes.Status401Unauthorized, BuildApiResponse(false, "Unauthorized", ua.Message)),
            KeyNotFoundException knf => (StatusCodes.Status404NotFound, BuildApiResponse(false, "Not Found", knf.Message)),
            InvalidOperationException ioe => (StatusCodes.Status400BadRequest, BuildApiResponse(false, "Invalid operation", ioe.Message)),
            _ => (StatusCodes.Status500InternalServerError, BuildApiResponse(false, "Unexpected error", "An unexpected error occurred"))
        };
    }

    private static ApiResponse BuildValidationApiResponse(ValidationException ve)
    {
        var errors = ve.Errors.Select(e => new ValidationErrorDetail
        {
            Error = e.ErrorCode,
            Detail = e.ErrorMessage
        });
        return new ApiResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };
    }

    private static ApiResponse BuildApiResponse(bool success, string message, string detail)
        => new ApiResponse { Success = success, Message = $"{message}: {detail}" };
}
