using FluentValidation;
using System.Text.Json;

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
            ValidationException ve => (StatusCodes.Status400BadRequest, BuildValidationError(ve)),
            UnauthorizedAccessException ua => (StatusCodes.Status401Unauthorized, new ErrorResponse("AuthenticationError", "Unauthorized", ua.Message)),
            KeyNotFoundException knf => (StatusCodes.Status404NotFound, new ErrorResponse("ResourceNotFound", "Not Found", knf.Message)),
            InvalidOperationException ioe => (StatusCodes.Status400BadRequest, new ErrorResponse("InvalidOperation", "Invalid operation", ioe.Message)),
            _ => (StatusCodes.Status500InternalServerError, new ErrorResponse("InternalServerError", "Unexpected error", "An unexpected error occurred"))
        };
    }

    private static object BuildValidationError(ValidationException ve)
    {
        var details = ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
        var joined = string.Join("; ", details);
        return new ErrorResponse("ValidationError", "Invalid input data", joined);
    }

    private sealed record ErrorResponse(string Type, string Error, string Detail);
}
