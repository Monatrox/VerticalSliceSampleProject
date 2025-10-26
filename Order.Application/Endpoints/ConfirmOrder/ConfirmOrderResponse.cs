using System.Net;
using VerticalSlice.Shared.Objects;

namespace Order.Application.Endpoints.ConfirmOrder;

public class ConfirmOrderResponse : ApiResponse
{
    public ConfirmOrderResponse()
    {
        Success = true;
        Status = 200;
    }

    public ConfirmOrderResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

