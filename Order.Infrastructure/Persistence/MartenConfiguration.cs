using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Marten.Events.Aggregation;
using Order.Domain.Events;
using Order.Infrastructure.Projections;

namespace Order.Infrastructure.Persistence;

public static class MartenConfiguration
{
    public static void ConfigureMarten(this StoreOptions options)
    {
        // Register event types
        options.Events.AddEventType<OrderCreated>();
        options.Events.AddEventType<OrderItemAdded>();
        options.Events.AddEventType<OrderConfirmed>();
        options.Events.AddEventType<OrderShipped>();

        // Configure projections (we'll add these next)
        options.Projections.Add<OrderProjection>(ProjectionLifecycle.Inline);
    }
}