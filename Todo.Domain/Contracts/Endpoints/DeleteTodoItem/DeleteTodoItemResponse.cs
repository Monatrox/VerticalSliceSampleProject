using System.Net;
using VerticalSlice.Shared.Objects;

namespace Todo.Domain.Contracts.Endpoints.DeleteTodoItem;

public class DeleteTodoItemResponse : ApiResponse<bool>
{
    public DeleteTodoItemResponse()
    {
    }

    public DeleteTodoItemResponse(bool data) : base(data)
    {
    }

    public DeleteTodoItemResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message)
    {
    }
}

