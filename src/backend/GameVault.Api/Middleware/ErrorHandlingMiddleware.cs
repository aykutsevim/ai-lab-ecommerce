using GameVault.Core.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace GameVault.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = exception switch
        {
            UnauthorizedAccessException => new
            {
                statusCode = (int)HttpStatusCode.Unauthorized,
                response = ApiResponse<object>.ErrorResponse(exception.Message)
            },
            InvalidOperationException => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                response = ApiResponse<object>.ErrorResponse(exception.Message)
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                response = ApiResponse<object>.ErrorResponse("An internal server error occurred", new List<string> { exception.Message })
            }
        };

        response.StatusCode = apiResponse.statusCode;
        await response.WriteAsync(JsonSerializer.Serialize(apiResponse.response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
