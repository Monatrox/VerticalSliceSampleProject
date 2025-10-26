using System.Net;
using VerticalSlice.Shared.Objects;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.GetWeatherForecast;

public class GetWeatherForecastResponse : ApiResponse<WeatherForecast>
{
    public GetWeatherForecastResponse(WeatherForecast forecast) : base(forecast)
    {
    }
    
    public GetWeatherForecastResponse()
    {
    }

    public GetWeatherForecastResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}