using FastEndpoints;
using JasperFx;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Services;
using Wolverine;
using Wolverine.Marten;

namespace Order.Application.Extensions;

public static class OrderServiceExtensions
{
    public static IServiceCollection AddOrderEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        
        return services;
    }
    
    // public static IServiceCollection AddOrderServices(this IHostApplicationBuilder builder,
    //     IConfiguration configuration)
    // {
    //     builder.Services.AddMarten(options =>
    //         {
    //             options.Connection(configuration.GetConnectionString("orderdb")!);
    //             options.AutoCreateSchemaObjects = AutoCreate.All;
    //             options.ConfigureMarten();
    //         })
    //         .IntegrateWithWolverine();
    //
    //     builder.Services.AddWolverine(opts =>
    //     {
    //         opts.Discovery.DisableConventionalDiscovery();
    //         opts.Discovery.IncludeAssembly(typeof(OrderServiceExtensions).Assembly);
    //     });
    //
    //     // Add Providers
    //     return builder.Services;
    // }

    public static IServiceCollection AddOrderServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
            {
                options.Connection(builder.Configuration.GetConnectionString("orderdb")!);
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.ConfigureMarten();
            })
            .IntegrateWithWolverine();

        builder.Services.AddWolverine(opts =>
        {
            opts.Discovery.DisableConventionalDiscovery();
            opts.Discovery.IncludeAssembly(typeof(OrderServiceExtensions).Assembly);
        });

        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();

        return builder.Services;
    }

    // public static IServiceCollection AddOrderApi(this IHostApplicationBuilder builder)
    // {
    //     // Just Marten for queries, no Wolverine message processing
    //     builder.Services.AddMarten(options =>
    //     {
    //         options.Connection(builder.Configuration.GetConnectionString("orderdb")!);
    //         options.AutoCreateSchemaObjects = AutoCreate.All;
    //         options.ConfigureMarten();
    //     });
    //
    //     builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();
    //
    //     builder.Services.AddFastEndpoints();
    //
    //     return builder.Services;
    // }
}