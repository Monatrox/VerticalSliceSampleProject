using System.Net;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.Services;

namespace Order.Application.Endpoints.GetOrder;

public class GetOrderEndpoint(IOrderQueryService service) 
    : Endpoint<GetOrderRequest, GetOrderResponse>
{
    public override void Configure()
    {
        Get("/orders/{id}");
        AllowAnonymous();
        Options(x => x.CacheOutput(policy => policy
            .Expire(TimeSpan.FromMinutes(5))
            .SetVaryByRouteValue("id")));
    }

    public override async Task<GetOrderResponse> ExecuteAsync(GetOrderRequest request, CancellationToken ct)
    {
        try
        {
            var data = await service.GetByIdAsync(request.Id, ct);
            
            return data is null 
                ? new GetOrderResponse(HttpStatusCode.NotFound)
                : new GetOrderResponse(data);
        }
        catch (Exception ex)
        {
            return new GetOrderResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
