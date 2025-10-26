using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Application.Extensions;

public static class TodoServiceExtensions
{
    public static IServiceCollection AddTodoEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        
        return services;
    }
}

