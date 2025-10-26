using Marten;
using Order.Domain.Enums;
using Order.Infrastructure.Projections;

namespace Order.Infrastructure.Services;

public interface IOrderQueryService
{
    Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderReadModel>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrderReadModel>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderReadModel>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
}

public sealed class OrderQueryService : IOrderQueryService
{
    private readonly IQuerySession _session;

    public OrderQueryService(IQuerySession session)
    {
        _session = session;
    }

    public Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _session.LoadAsync<OrderReadModel>(id, ct);
    }

    public Task<IReadOnlyList<OrderReadModel>> GetAllAsync(CancellationToken ct = default)
    {
        return _session.Query<OrderReadModel>()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<IReadOnlyList<OrderReadModel>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default)
    {
        return _session.Query<OrderReadModel>()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<IReadOnlyList<OrderReadModel>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default)
    {
        return _session.Query<OrderReadModel>()
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }
}