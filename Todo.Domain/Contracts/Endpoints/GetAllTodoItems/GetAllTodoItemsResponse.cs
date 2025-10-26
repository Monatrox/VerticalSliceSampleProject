using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Domain.Contracts.Endpoints.GetAllTodos;

public class GetAllTodoItemsResponse : ApiResponse<IEnumerable<TodoItem>>
{
    public GetAllTodoItemsResponse(IEnumerable<TodoItem> data) : base(data)
    {
    }
    
    public GetAllTodoItemsResponse()
    {
    }

    public GetAllTodoItemsResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

