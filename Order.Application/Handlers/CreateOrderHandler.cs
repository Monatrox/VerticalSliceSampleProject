using Marten;

namespace Order.Application.Handlers;

public record CreateOrderCommand(string CustomerId);

public class CreateOrderHandler
{
    public async Task<Guid> Handle(
        CreateOrderCommand command,
        IDocumentSession session,
        CancellationToken ct)
    {
        var order = Domain.Objects.Order.Create(command.CustomerId);
        var events = order.GetUncommittedEvents();

        session.Events.StartStream<Domain.Objects.Order>(order.Id, events.ToArray());
        await session.SaveChangesAsync(ct);

        return order.Id;
    }
}