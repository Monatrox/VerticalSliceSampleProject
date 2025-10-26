using System.Net;
using FastEndpoints;
using Order.Infrastructure.Services;

namespace Order.Application.Endpoints.AddOrderItem;

public class AddOrderItemEndpoint(IOrderService service) 
    : Endpoint<AddOrderItemRequest, AddOrderItemResponse>
{
    public override void Configure()
    {
        Post("/orders/{orderId}/items");
        AllowAnonymous();
    }

    public override async Task<AddOrderItemResponse> ExecuteAsync(AddOrderItemRequest request, CancellationToken ct)
    {
        try
        {
            await service.AddItemAsync(
                request.OrderId, 
                request.ProductId, 
                request.ProductName, 
                request.Quantity, 
                request.UnitPrice, 
                ct);
            
            return new AddOrderItemResponse();
        }
        catch (Exception ex)
        {
            return new AddOrderItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

