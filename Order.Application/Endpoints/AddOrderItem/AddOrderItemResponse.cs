using System.Net;
using VerticalSlice.Shared.Objects;

namespace Order.Application.Endpoints.AddOrderItem;

public class AddOrderItemResponse : ApiResponse
{
    public AddOrderItemResponse()
    {
        Success = true;
        Status = 200;
    }

    public AddOrderItemResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

