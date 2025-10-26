namespace Weather.Public;

public sealed record WeatherForecastDto(DateOnly Date, int TemperatureC, string Summary);