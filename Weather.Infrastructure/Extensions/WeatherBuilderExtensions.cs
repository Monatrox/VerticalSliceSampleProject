using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Weather.Infrastructure.Persistence;
using Weather.Infrastructure.Services;

namespace Weather.Infrastructure.Extensions;

public static class WeatherBuilderExtensions
{
    public static IServiceCollection AddWeatherServices(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<WeatherDbContext>(
            "weatherdb",
            configureDbContextOptions: options =>
            {
                options.UseNpgsql(npgsqlBuilder =>
                {
                    npgsqlBuilder.MigrationsAssembly("Weather.Infrastructure");
                    npgsqlBuilder.MigrationsHistoryTable("__EFMigrationsHistory", "weather");
                    npgsqlBuilder.EnableRetryOnFailure(2, TimeSpan.FromSeconds(2), null);
                });
            });
        
        builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        
        return builder.Services;
    }
}