using System.Net;
using FastEndpoints;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints.CompleteTodoItem;

public class CompleteTodoItemEndpoint : Endpoint<CompleteTodoItemRequest, CompleteTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public CompleteTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Patch("/todos/{id}/complete");
        AllowAnonymous();
    }

    public override async Task<CompleteTodoItemResponse> ExecuteAsync(CompleteTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var data = await _service.CompleteAsync(request.Id, ct);
            
            return data is null 
                ? new CompleteTodoItemResponse(HttpStatusCode.NotFound)
                : new CompleteTodoItemResponse(data);
        }
        catch (InvalidOperationException ex)
        {
            return new CompleteTodoItemResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            return new CompleteTodoItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

