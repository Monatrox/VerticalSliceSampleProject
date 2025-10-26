using System.Net;
using FastEndpoints;
using Weather.Infrastructure.Services;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.CreateWeatherForecast;

public class CreateWeatherForecastEndpoint(IWeatherForecastService service)
    : Endpoint<CreateWeatherForecastRequest, CreateWeatherForecastResponse>
{
    public override void Configure()
    {
        Post("/weather");
        AllowAnonymous();
    }

    public override async Task<CreateWeatherForecastResponse> ExecuteAsync(CreateWeatherForecastRequest request, CancellationToken ct)
    {
        try
        {
            var forecast = new WeatherForecast
            {
                LocationId = request.LocationId,
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            var data = await service.CreateAsync(forecast, ct);
            
            return new CreateWeatherForecastResponse(data);
        }
        catch (InvalidOperationException ex)
        {
            return new CreateWeatherForecastResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            return new CreateWeatherForecastResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}