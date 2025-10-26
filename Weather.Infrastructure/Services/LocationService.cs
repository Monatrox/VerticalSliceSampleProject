using Microsoft.EntityFrameworkCore;
using Weather.Domain.Objects;
using Weather.Infrastructure.Persistence;

namespace Weather.Infrastructure.Services;

public interface ILocationService
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<Location?> GetByCityAndStateAsync(string city, string state, CancellationToken cancellationToken = default);
    Task<Location> CreateAsync(Location location, CancellationToken cancellationToken = default);
    Task<Location?> UpdateAsync(Guid id, Location location, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class LocationService : ILocationService
{
    private readonly WeatherDbContext _context;

    public LocationService(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .OrderBy(l => l.State)
            .ThenBy(l => l.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .Where(l => l.City.ToLower() == city.ToLower())
            .OrderBy(l => l.State)
            .ToListAsync(cancellationToken);
    }

    public async Task<Location?> GetByCityAndStateAsync(string city, string state, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.City.ToLower() == city.ToLower() 
                                   && l.State.ToLower() == state.ToLower(), 
                                   cancellationToken);
    }

    public async Task<Location> CreateAsync(Location location, CancellationToken cancellationToken = default)
    {
        // Check for duplicate city/state combination
        var exists = await GetByCityAndStateAsync(location.City, location.State, cancellationToken);
        if (exists != null)
        {
            throw new InvalidOperationException($"Location '{location.City}, {location.State}' already exists.");
        }

        _context.Locations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);
        
        return location;
    }

    public async Task<Location?> UpdateAsync(Guid id, Location location, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        
        if (existing == null)
        {
            return null;
        }

        // Check if the new city/state combo conflicts with another location
        var duplicate = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id != id 
                                   && l.City.ToLower() == location.City.ToLower() 
                                   && l.State.ToLower() == location.State.ToLower(), 
                                   cancellationToken);
        
        if (duplicate != null)
        {
            throw new InvalidOperationException($"Location '{location.City}, {location.State}' already exists.");
        }

        existing.City = location.City;
        existing.State = location.State;

        await _context.SaveChangesAsync(cancellationToken);
        
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        
        if (location == null)
        {
            return false;
        }

        // Check if location has any weather forecasts
        var hasForecasts = await _context.WeatherForecasts
            .AnyAsync(w => w.LocationId == id, cancellationToken);
        
        if (hasForecasts)
        {
            throw new InvalidOperationException($"Cannot delete location '{location.City}, {location.State}' because it has associated weather forecasts.");
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}