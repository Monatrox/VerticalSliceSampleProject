namespace Todo.Public;

public sealed record TodoItemDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    DateTime? DueDate,
    int Priority);

