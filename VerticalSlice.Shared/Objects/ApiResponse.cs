using System.Net;

namespace VerticalSlice.Shared.Objects;

public class ApiResponse
{
    public bool Success { get; set; }
    public int Status { get; set; } = 200;
    public ApiError? Error { get; set; }
    public ApiMetadata? Metadata { get; set; } = new();

    public ApiResponse()
    {
    }

    public ApiResponse(HttpStatusCode statusCode, string? message = null)
    {
        Success = false;
        Status = (int) statusCode;
        Error = new ApiError(statusCode, message);
    }
}

public class ApiResponse<T> : ApiResponse
{
    public ApiResponse()
    {
    }

    public ApiResponse(T data)
    {
        Success = true;
        Status = 200;
        Data = data;
    }

    public ApiResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }

    public T? Data { get; set; } = default!;
}