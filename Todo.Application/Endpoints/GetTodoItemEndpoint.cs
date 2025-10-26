using System.Net;
using FastEndpoints;
using Todo.Domain.Contracts.Endpoints.GetTodoItem;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints;

public class GetTodoItemEndpoint(ITodoItemService service) : Endpoint<GetTodoItemRequest, GetTodoItemResponse>
{
    public override void Configure()
    {
        Get("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<GetTodoItemResponse> ExecuteAsync(GetTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var data = await service.GetByIdAsync(request.Id, ct);
        
            return data is null 
                ? new GetTodoItemResponse(HttpStatusCode.NotFound)
                : new GetTodoItemResponse(data);
        }
        catch (Exception ex)
        {
            return new GetTodoItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

