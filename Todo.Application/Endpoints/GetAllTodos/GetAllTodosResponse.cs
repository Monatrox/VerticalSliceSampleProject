using System.Net;
using VerticalSlice.Shared.Objects;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.GetAllTodos;

public class GetAllTodosResponse : ApiResponse<IEnumerable<TodoItem>>
{
    public GetAllTodosResponse(IEnumerable<TodoItem> data) : base(data)
    {
    }
    
    public GetAllTodosResponse()
    {
    }

    public GetAllTodosResponse(HttpStatusCode code, string? message = null) : base(code, message)
    {
    }
}

