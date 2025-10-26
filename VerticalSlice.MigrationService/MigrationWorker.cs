using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Persistence;
using Weather.Infrastructure.Persistence;

namespace VerticalSlice.MigrationService;

public class MigrationWorker(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is {IsClass: true, IsAbstract: false} && type.IsSubclassOf(typeof(DbContext)));

        foreach (var contextType in dbContextTypes)
        {
            if (scope.ServiceProvider.GetService(contextType) is DbContext dbContext)
            {
                // Extract schema name from context type (e.g., "WeatherDbContext" -> "weather")
                var schemaName = contextType.Name.Replace("DbContext", "").ToLowerInvariant();

                // Create schema if it doesn't exist
                await dbContext.Database.ExecuteSqlRawAsync(
                    $"CREATE SCHEMA IF NOT EXISTS {schemaName}",
                    stoppingToken);

                // Now run migrations
                await dbContext.Database.MigrateAsync(stoppingToken);
            }
        }

        lifetime.StopApplication();
    }
}