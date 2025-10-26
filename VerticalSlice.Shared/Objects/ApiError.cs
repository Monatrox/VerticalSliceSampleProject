using System.Net;

namespace VerticalSlice.Shared.Objects;

public class ApiError
{
    public string Error { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? Code { get; set; }
    
    public Dictionary<string, string>? Errors { get; set; }
    
    public ApiError(HttpStatusCode statusCode, string? message = null)
    {
        Error = nameof(statusCode);
        Code = statusCode.ToString();
        Message = message ?? GetDefaultMessage(statusCode);
    }

    private static string GetDefaultMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.BadRequest => "Invalid request",
            HttpStatusCode.Conflict => "Resource conflict",
            HttpStatusCode.Unauthorized => "Authentication required",
            HttpStatusCode.Forbidden => "Access denied",
            HttpStatusCode.InternalServerError => "An error occurred",
            _ => "An error occurred"
        };
    }
}