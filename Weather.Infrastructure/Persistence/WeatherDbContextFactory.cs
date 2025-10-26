using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Weather.Infrastructure.Persistence;

public sealed class WeatherDbContextFactory : IDesignTimeDbContextFactory<WeatherDbContext>
{
    public WeatherDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WeatherDbContext>();
        // This connection string is for design-time use only. It needs to be well-formed, but doesn't need to work.
        optionsBuilder.UseNpgsql("Host=localhost;Database=weatherdb;Username=weatheruser;Password=weatherpassword");
        
        return new WeatherDbContext(optionsBuilder.Options);
    }
}