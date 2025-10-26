using Marten;

namespace Order.Application.Handlers;

public record AddOrderItemCommand(
    Guid OrderId,
    string ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

public class AddOrderItemHandler
{
    public async Task Handle(
        AddOrderItemCommand command,
        IDocumentSession session,
        CancellationToken ct)
    {
        var order = await session.Events.AggregateStreamAsync<Domain.Objects.Order>(command.OrderId, token: ct);

        if (order == null)
            throw new InvalidOperationException($"Order {command.OrderId} not found");

        order.AddItem(command.ProductId, command.ProductName, command.Quantity, command.UnitPrice);
        var events = order.GetUncommittedEvents();

        session.Events.Append(command.OrderId, events.ToArray());
        await session.SaveChangesAsync(ct);
    }
}