using System.Net;
using FastEndpoints;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints.DeleteTodoItem;

public class DeleteTodoItemEndpoint : Endpoint<DeleteTodoItemRequest, DeleteTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public DeleteTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<DeleteTodoItemResponse> ExecuteAsync(DeleteTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var success = await _service.DeleteAsync(request.Id, ct);
            
            return success 
                ? new DeleteTodoItemResponse(true)
                : new DeleteTodoItemResponse(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            return new DeleteTodoItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

