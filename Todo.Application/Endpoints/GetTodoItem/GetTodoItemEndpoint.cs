using System.Net;
using FastEndpoints;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints.GetTodoItem;

public class GetTodoItemEndpoint : Endpoint<GetTodoItemRequest, GetTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public GetTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<GetTodoItemResponse> ExecuteAsync(GetTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var data = await _service.GetByIdAsync(request.Id, ct);
        
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

