using System.Net;
using FastEndpoints;
using Order.Infrastructure.Services;

namespace Order.Application.Endpoints.ShipOrder;

public class ShipOrderEndpoint(IOrderService service) 
    : Endpoint<ShipOrderRequest, ShipOrderResponse>
{
    public override void Configure()
    {
        Post("/orders/{orderId}/ship");
        AllowAnonymous();
    }

    public override async Task<ShipOrderResponse> ExecuteAsync(ShipOrderRequest request, CancellationToken ct)
    {
        try
        {
            await service.ShipOrderAsync(request.OrderId, ct);
            return new ShipOrderResponse();
        }
        catch (Exception ex)
        {
            return new ShipOrderResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

