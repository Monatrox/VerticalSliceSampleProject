using System.Net;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using VerticalSlice.Shared.Objects;
using Weather.Infrastructure.Services;

namespace Weather.Application.Endpoints.GetWeatherForecast;

public class GetWeatherForecastEndpoint : Endpoint<GetWeatherForecastRequest, GetWeatherForecastResponse>
{
    private readonly IWeatherForecastService _service;

    public GetWeatherForecastEndpoint(IWeatherForecastService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/weather/{id}");
        AllowAnonymous();
        Options(x => x.CacheOutput(policy => policy
            .Expire(TimeSpan.FromMinutes(5))
            .SetVaryByRouteValue("id")));
    }

    public override async Task<GetWeatherForecastResponse> ExecuteAsync(GetWeatherForecastRequest request, CancellationToken ct)
    {
        try
        {
            var data = await _service.GetByIdAsync(request.Id, ct);
        
            return data is null 
                ? new GetWeatherForecastResponse(HttpStatusCode.NotFound)
                : new GetWeatherForecastResponse(data);
        }
        catch (Exception ex)
        {
            return new GetWeatherForecastResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}