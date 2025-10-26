using Order.Domain.Enums;
using Order.Domain.Events;
using Marten.Events.Aggregation;

namespace Order.Infrastructure.Projections;

public class OrderProjection : SingleStreamProjection<OrderReadModel, Guid>
{
    public OrderReadModel Create(OrderCreated @event)
    {
        return new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            Status = OrderStatus.Draft,
            CreatedAt = @event.CreatedAt
        };
    }

    public void Apply(OrderItemAdded @event, OrderReadModel order)
    {
        order.Items.Add(new OrderItemReadModel
        {
            ProductId = @event.ProductId,
            ProductName = @event.ProductName,
            Quantity = @event.Quantity,
            UnitPrice = @event.UnitPrice
        });
    }

    public void Apply(OrderConfirmed @event, OrderReadModel order)
    {
        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = @event.ConfirmedAt;
    }

    public void Apply(OrderShipped @event, OrderReadModel order)
    {
        order.Status = OrderStatus.Shipped;
        order.ShippedAt = @event.ShippedAt;
    }
}