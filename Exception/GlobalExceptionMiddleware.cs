using System.Text.Json;

namespace ShippingService.Middleware;

/// <summary>
/// Catches all unhandled exceptions and returns a consistent JSON error response.
/// Registered before all other middleware in Program.cs so it wraps the entire pipeline.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,
                                     ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteErrorResponse(context, ex);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = ex switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message, (Dictionary<string, string[]>?)null),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message, null),
            ArgumentException => (StatusCodes.Status400BadRequest, ex.Message, null),
            _ => (StatusCodes.Status500InternalServerError,
                                       "An unexpected error occurred.", null)
        };

        context.Response.StatusCode = statusCode;

        var body = new ErrorResponse(statusCode, message, errors);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}

/// <summary>
/// Consistent error response shape across all endpoints.
/// Matches what FluentValidation's auto-validation also produces (extended via 'errors').
/// </summary>
public record ErrorResponse(
    int Status,
    string Message,
    Dictionary<string, string[]>? Errors
);