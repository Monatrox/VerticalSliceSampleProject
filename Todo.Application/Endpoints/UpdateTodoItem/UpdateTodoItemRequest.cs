namespace Todo.Application.Endpoints.UpdateTodoItem;

public class UpdateTodoItemRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
    public bool IsCompleted { get; set; }
}

