using System.Net;
using VerticalSlice.Shared.Objects;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.CreateLocation;

public class CreateLocationResponse : ApiResponse<Location>
{
    public CreateLocationResponse()
    {
    }

    public CreateLocationResponse(Location data) : base(data)
    {
    }

    public CreateLocationResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}