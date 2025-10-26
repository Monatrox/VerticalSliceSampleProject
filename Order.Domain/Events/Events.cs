namespace Order.Domain.Events;
// This could be split into multiple files, (one per event) but for demonstration that's a bit unnecessary

public record OrderCreated(Guid OrderId, string CustomerId, DateTime CreatedAt);
public record OrderItemAdded(Guid OrderId, string ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record OrderConfirmed(Guid OrderId, DateTime ConfirmedAt);
public record OrderShipped(Guid OrderId, DateTime ShippedAt);