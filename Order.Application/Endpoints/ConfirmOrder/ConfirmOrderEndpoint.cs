using System.Net;
using FastEndpoints;
using Order.Infrastructure.Services;

namespace Order.Application.Endpoints.ConfirmOrder;

public class ConfirmOrderEndpoint(IOrderService service) 
    : Endpoint<ConfirmOrderRequest, ConfirmOrderResponse>
{
    public override void Configure()
    {
        Post("/orders/{orderId}/confirm");
        AllowAnonymous();
    }

    public override async Task<ConfirmOrderResponse> ExecuteAsync(ConfirmOrderRequest request, CancellationToken ct)
    {
        try
        {
            await service.ConfirmOrderAsync(request.OrderId, ct);
            return new ConfirmOrderResponse();
        }
        catch (Exception ex)
        {
            return new ConfirmOrderResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

