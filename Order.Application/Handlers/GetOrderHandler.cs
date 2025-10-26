using Marten;
using Order.Infrastructure.Projections;

namespace Order.Application.Handlers;

public record GetOrderQuery(Guid OrderId);

public class GetOrderHandler
{
    public async Task<OrderReadModel?> Handle(
        GetOrderQuery query,
        IDocumentSession session,
        CancellationToken ct)
    {
        return await session.Query<OrderReadModel>()
            .FirstOrDefaultAsync(x => x.Id == query.OrderId, ct);
    }
}