using System.Net;
using VerticalSlice.Shared.Objects;

namespace Order.Application.Endpoints.CreateOrder;

public class CreateOrderResponse : ApiResponse<Guid>
{
    public CreateOrderResponse(Guid orderId) : base(orderId)
    {
    }
    
    public CreateOrderResponse()
    {
    }

    public CreateOrderResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

