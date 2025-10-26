using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.GetTodoItem;

public class GetTodoItemResponse : ApiResponse<TodoItem>
{
    public GetTodoItemResponse(TodoItem todoItem) : base(todoItem)
    {
    }
    
    public GetTodoItemResponse()
    {
    }

    public GetTodoItemResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

