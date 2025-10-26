using Microsoft.EntityFrameworkCore;
using Weather.Domain.Objects;
using Weather.Infrastructure.Persistence;

namespace Weather.Infrastructure.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherForecast>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<WeatherForecast> CreateAsync(WeatherForecast forecast, CancellationToken cancellationToken = default);
    Task<WeatherForecast?> UpdateAsync(Guid id, WeatherForecast forecast, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class WeatherForecastService : IWeatherForecastService
{
    private readonly WeatherDbContext _context;

    public WeatherForecastService(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .Include(w => w.Location)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .Include(w => w.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherForecast>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .Include(w => w.Location)
            .Where(w => w.LocationId == locationId)
            .OrderByDescending(w => w.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherForecast> CreateAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        // Validate that the location exists
        var locationExists = await _context.Locations
            .AnyAsync(l => l.Id == forecast.LocationId, cancellationToken);
        
        if (!locationExists)
        {
            throw new InvalidOperationException($"Location with ID {forecast.LocationId} does not exist.");
        }

        _context.WeatherForecasts.Add(forecast);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload with location data
        return (await GetByIdAsync(forecast.Id, cancellationToken))!;
    }

    public async Task<WeatherForecast?> UpdateAsync(Guid id, WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        var existing = await _context.WeatherForecasts
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        
        if (existing == null)
        {
            return null;
        }

        // Validate location if it's being changed
        if (existing.LocationId != forecast.LocationId)
        {
            var locationExists = await _context.Locations
                .AnyAsync(l => l.Id == forecast.LocationId, cancellationToken);
            
            if (!locationExists)
            {
                throw new InvalidOperationException($"Location with ID {forecast.LocationId} does not exist.");
            }
        }

        existing.LocationId = forecast.LocationId;
        existing.Date = forecast.Date;
        existing.TemperatureC = forecast.TemperatureC;
        existing.Summary = forecast.Summary;

        await _context.SaveChangesAsync(cancellationToken);
        
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var forecast = await _context.WeatherForecasts
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        
        if (forecast == null)
        {
            return false;
        }

        _context.WeatherForecasts.Remove(forecast);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}