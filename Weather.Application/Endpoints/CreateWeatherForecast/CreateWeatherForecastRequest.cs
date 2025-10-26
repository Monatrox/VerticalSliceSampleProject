namespace Weather.Application.Endpoints.CreateWeatherForecast;

public class CreateWeatherForecastRequest
{
    public Guid LocationId { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}