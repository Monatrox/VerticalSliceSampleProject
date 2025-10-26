using System.Reflection;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Public.Abstractions;
using Todo.Public.Providers;
using Weather.Infrastructure.Persistence;
using Weather.Infrastructure.Services;

namespace Weather.Application.Extensions;

public static class WeatherServiceExtensions
{
    public static IServiceCollection AddWeatherEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        
        return services;
    }
    
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
        
        // Add Providers for Integration 
        builder.Services.AddScoped<ITodoItemProvider, TodoItemProvider>();
        
        return builder.Services;
    }
}