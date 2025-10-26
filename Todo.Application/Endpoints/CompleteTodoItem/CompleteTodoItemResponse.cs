using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.CompleteTodoItem;

public class CompleteTodoItemResponse : ApiResponse<TodoItem>
{
    public CompleteTodoItemResponse()
    {
    }

    public CompleteTodoItemResponse(TodoItem data) : base(data)
    {
    }

    public CompleteTodoItemResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}

