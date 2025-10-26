namespace Weather.Public.Abstractions;

public interface IWeatherForecastProvider
{
    ValueTask<IReadOnlyList<WeatherForecastDto>> GetAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);
}