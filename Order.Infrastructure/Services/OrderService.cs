using Marten;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure.Services;

public interface IOrderService
{
    Task<Guid> CreateOrderAsync(string customerId, CancellationToken ct = default);
    Task AddItemAsync(Guid orderId, string productId, string productName, int quantity, decimal unitPrice, CancellationToken ct = default);
    Task ConfirmOrderAsync(Guid orderId, CancellationToken ct = default);
    Task ShipOrderAsync(Guid orderId, CancellationToken ct = default);
}

public sealed class OrderService : IOrderService
{
    private readonly IDocumentSession _session;

    public OrderService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Guid> CreateOrderAsync(string customerId, CancellationToken ct = default)
    {
        var orderId = Guid.NewGuid();
        var @event = new OrderCreated(orderId, customerId, DateTime.UtcNow);

        _session.Events.StartStream<Domain.Objects.Order>(orderId, @event);
        await _session.SaveChangesAsync(ct);

        return orderId;
    }

    public async Task AddItemAsync(Guid orderId, string productId, string productName, int quantity, decimal unitPrice, CancellationToken ct = default)
    {
        var @event = new OrderItemAdded(orderId, productId, productName, quantity, unitPrice);

        _session.Events.Append(orderId, @event);
        await _session.SaveChangesAsync(ct);
    }

    public async Task ConfirmOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var @event = new OrderConfirmed(orderId, DateTime.UtcNow);

        _session.Events.Append(orderId, @event);
        await _session.SaveChangesAsync(ct);
    }

    public async Task ShipOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var @event = new OrderShipped(orderId, DateTime.UtcNow);

        _session.Events.Append(orderId, @event);
        await _session.SaveChangesAsync(ct);
    }
}