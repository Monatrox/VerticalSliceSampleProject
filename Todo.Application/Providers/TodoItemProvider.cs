using Todo.Infrastructure.Services;
using Todo.Public.Abstractions;

namespace Todo.Public.Providers;

public sealed class TodoItemProvider(ITodoItemService todoItemService) : ITodoItemProvider
{
    public async ValueTask<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await todoItemService.GetAllAsync(ct);
        return MapToDtos(items);
    }

    public async ValueTask<IReadOnlyList<TodoItemDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var items = await todoItemService.GetActiveAsync(ct);
        return MapToDtos(items);
    }

    public async ValueTask<IReadOnlyList<TodoItemDto>> GetCompletedAsync(CancellationToken ct = default)
    {
        var items = await todoItemService.GetCompletedAsync(ct);
        return MapToDtos(items);
    }

    public async ValueTask<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await todoItemService.GetByIdAsync(id, ct);
        return item == null ? null : MapToDto(item);
    }

    private static TodoItemDto MapToDto(Domain.Objects.TodoItem item) => new(item.Id,
        item.Title,
        item.Description,
        item.IsCompleted,
        item.CreatedAt,
        item.CompletedAt,
        item.DueDate,
        item.Priority);

    private static IReadOnlyList<TodoItemDto> MapToDtos(IEnumerable<Domain.Objects.TodoItem> items) =>
        items.Select(MapToDto).ToList();
}