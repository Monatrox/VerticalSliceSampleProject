using System.Net;
using VerticalSlice.Shared.Objects;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.CreateWeatherForecast;

public class CreateWeatherForecastResponse : ApiResponse<WeatherForecast>
{
    public CreateWeatherForecastResponse()
    {
    }

    public CreateWeatherForecastResponse(WeatherForecast data) : base(data)
    {
    }

    public CreateWeatherForecastResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}