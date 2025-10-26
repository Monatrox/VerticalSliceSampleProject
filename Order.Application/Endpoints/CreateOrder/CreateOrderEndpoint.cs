using System.Net;
using FastEndpoints;
using Order.Infrastructure.Services;

namespace Order.Application.Endpoints.CreateOrder;

public class CreateOrderEndpoint(IOrderService service) 
    : Endpoint<CreateOrderRequest, CreateOrderResponse>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
    }

    public override async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken ct)
    {
        try
        {
            var orderId = await service.CreateOrderAsync(request.CustomerId, ct);
            return new CreateOrderResponse(orderId);
        }
        catch (Exception ex)
        {
            return new CreateOrderResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}



