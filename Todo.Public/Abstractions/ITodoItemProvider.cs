namespace Todo.Public.Abstractions;

public interface ITodoItemProvider
{
    ValueTask<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken ct = default);
    ValueTask<IReadOnlyList<TodoItemDto>> GetActiveAsync(CancellationToken ct = default);
    ValueTask<IReadOnlyList<TodoItemDto>> GetCompletedAsync(CancellationToken ct = default);
    ValueTask<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

