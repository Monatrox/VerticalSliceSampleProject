using Order.Domain.Enums;
using Order.Domain.Events;

namespace Order.Domain.Objects;

public class Order
{
    private readonly List<object> _uncommittedEvents = new();
    public IEnumerable<object> GetUncommittedEvents() => _uncommittedEvents;
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();
    
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);

    // Private constructor for EF Core
    private Order() { }
    
    private void RaiseEvent(object @event)
    {
        Apply((dynamic)@event);
        _uncommittedEvents.Add(@event);
    }

    public static Order Create(string customerId)
    {
        var order = new Order();
        order.RaiseEvent(new OrderCreated(Guid.NewGuid(), customerId, DateTime.UtcNow));
        return order;
    }

    public void AddItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Can only add items to draft orders");

        RaiseEvent(new OrderItemAdded(Id, productId, productName, quantity, unitPrice));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be confirmed");

        RaiseEvent(new OrderConfirmed(Id, DateTime.UtcNow));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        RaiseEvent(new OrderShipped(Id, DateTime.UtcNow));
    }
    
    public void Apply(OrderCreated @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
        Status = OrderStatus.Draft;
        CreatedAt = @event.CreatedAt;
    }

    public void Apply(OrderItemAdded @event)
    {
        Items.Add(OrderItem.Create(Id, @event.ProductId, @event.ProductName, @event.Quantity, @event.UnitPrice));
    }

    public void Apply(OrderConfirmed @event)
    {
        Status = OrderStatus.Confirmed;
        ConfirmedAt = @event.ConfirmedAt;
    }

    public void Apply(OrderShipped @event)
    {
        Status = OrderStatus.Shipped;
        ShippedAt = @event.ShippedAt;
    }
}