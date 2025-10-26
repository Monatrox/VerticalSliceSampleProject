namespace Todo.Application.Endpoints.CreateTodoItem;

public class CreateTodoItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 2; // Default to Medium
}

