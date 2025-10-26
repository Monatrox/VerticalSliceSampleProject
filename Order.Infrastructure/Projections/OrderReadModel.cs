using Order.Domain.Enums;

namespace Order.Infrastructure.Projections;

public class OrderReadModel
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public List<OrderItemReadModel> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
}