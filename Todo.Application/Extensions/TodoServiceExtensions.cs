using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Services;

namespace Todo.Application.Extensions;

public static class TodoServiceExtensions
{
    public static IServiceCollection AddTodoEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        
        return services;
    }
    
    public static IServiceCollection AddTodoServices(this IHostApplicationBuilder builder)
    {
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
        
        builder.Services.AddScoped<ITodoItemService, TodoItemService>();
        
        return builder.Services;
    }
}

