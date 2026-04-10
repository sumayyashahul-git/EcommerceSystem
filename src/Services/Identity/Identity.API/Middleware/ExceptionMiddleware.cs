using System.Net;
using System.Text.Json;
using SharedKernel.Exceptions;

namespace Identity.API.Middleware;

/// <summary>
/// Global exception handler — catches ALL unhandled exceptions.
/// Converts them to consistent JSON error responses.
/// 
/// Without this: exceptions bubble up and return ugly 500 errors
/// With this: every error returns clean JSON with correct status code
/// 
/// Registered in Program.cs — runs on every request
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass request to next middleware/controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            // Convert to JSON response
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        // Determine status code based on exception type
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,           // 404
            ValidationException => HttpStatusCode.BadRequest,       // 400
            AppException appEx => (HttpStatusCode)appEx.StatusCode, // custom
            _ => HttpStatusCode.InternalServerError                  // 500
        };

        // Get error messages
        var errors = exception is ValidationException validationEx
            ? validationEx.Errors
            : new List<string> { exception.Message };

        // Build response object
        var response = new
        {
            status = (int)statusCode,
            message = exception.Message,
            errors = errors
        };

        // Write JSON response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }
}