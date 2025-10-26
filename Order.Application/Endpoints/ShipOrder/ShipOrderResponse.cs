using System.Net;
using VerticalSlice.Shared.Objects;

namespace Order.Application.Endpoints.ShipOrder;

public class ShipOrderResponse : ApiResponse
{
    public ShipOrderResponse()
    {
        Success = true;
        Status = 200;
    }

    public ShipOrderResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

