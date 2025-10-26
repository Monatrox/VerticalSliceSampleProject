using System.Reflection;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Weather.Application.Extensions;

public static class WeatherServiceExtensions
{
    public static IServiceCollection AddWeatherEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        
        return services;
    }
}