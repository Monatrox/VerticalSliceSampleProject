using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Domain.Contracts.Endpoints.CreateTodoItem;

public class CreateTodoItemResponse : ApiResponse<TodoItem>
{
    public CreateTodoItemResponse()
    {
    }

    public CreateTodoItemResponse(TodoItem data) : base(data)
    {
    }

    public CreateTodoItemResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}

