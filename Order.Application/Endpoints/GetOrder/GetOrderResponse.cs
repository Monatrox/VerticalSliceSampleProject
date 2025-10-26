using System.Net;
using Order.Infrastructure.Projections;
using VerticalSlice.Shared.Objects;

namespace Order.Application.Endpoints.GetOrder;

public class GetOrderResponse : ApiResponse<OrderReadModel>
{
    public GetOrderResponse(OrderReadModel order) : base(order)
    {
    }
    
    public GetOrderResponse()
    {
    }

    public GetOrderResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

