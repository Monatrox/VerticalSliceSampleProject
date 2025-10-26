using System.Net;
using FastEndpoints;
using Todo.Domain.Contracts.Endpoints.DeleteTodoItem;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints;

public class DeleteTodoItemEndpoint(ITodoItemService service) : Endpoint<DeleteTodoItemRequest, DeleteTodoItemResponse>
{
    public override void Configure()
    {
        Delete("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<DeleteTodoItemResponse> ExecuteAsync(DeleteTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var success = await service.DeleteAsync(request.Id, ct);
            
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

