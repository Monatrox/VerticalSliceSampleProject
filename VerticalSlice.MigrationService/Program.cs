using Microsoft.EntityFrameworkCore;
using Npgsql;
using Todo.Infrastructure.Persistence;
using VerticalSlice.MigrationService;
using Weather.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<MigrationWorker>();

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

builder.AddNpgsqlDbContext<TodoDbContext>(
    "tododb",
    configureDbContextOptions: options =>
    {
        options.UseNpgsql(npgsqlBuilder =>
        {
            npgsqlBuilder.MigrationsAssembly("Todo.Infrastructure");
            npgsqlBuilder.MigrationsHistoryTable("__EFMigrationsHistory", "todo");
            npgsqlBuilder.EnableRetryOnFailure(2, TimeSpan.FromSeconds(2), null);
        });
    });

var host = builder.Build();
host.Run();