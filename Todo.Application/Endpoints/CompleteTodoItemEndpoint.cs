using System.Net;
using FastEndpoints;
using Todo.Domain.Contracts.Endpoints.CompleteTodoItem;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints;

public class CompleteTodoItemEndpoint(ITodoItemService service)
    : Endpoint<CompleteTodoItemRequest, CompleteTodoItemResponse>
{
    public override void Configure()
    {
        Patch("/todos/{id}/complete");
        AllowAnonymous();
    }

    public override async Task<CompleteTodoItemResponse> ExecuteAsync(CompleteTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var data = await service.CompleteAsync(request.Id, ct);
            
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

