using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.UpdateTodoItem;

public class UpdateTodoItemResponse : ApiResponse<TodoItem>
{
    public UpdateTodoItemResponse()
    {
    }

    public UpdateTodoItemResponse(TodoItem data) : base(data)
    {
    }

    public UpdateTodoItemResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}

