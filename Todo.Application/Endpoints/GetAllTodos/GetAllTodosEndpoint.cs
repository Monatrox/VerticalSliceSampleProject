using System.Net;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints.GetAllTodos;

public class GetAllTodosEndpoint : Endpoint<GetAllTodosRequest, GetAllTodosResponse>
{
    private readonly ITodoItemService _service;

    public GetAllTodosEndpoint(ITodoItemService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/todos");
        AllowAnonymous();
        Options(x => x.CacheOutput(policy => policy
            .Expire(TimeSpan.FromMinutes(5)))
        );
    }

    public override async Task<GetAllTodosResponse> ExecuteAsync(GetAllTodosRequest request, CancellationToken ct)
    {
        try
        {
            IEnumerable<Domain.Objects.TodoItem> data;
            
            if (request.IsCompleted.HasValue)
            {
                data = request.IsCompleted.Value 
                    ? await _service.GetCompletedAsync(ct)
                    : await _service.GetActiveAsync(ct);
            }
            else
            {
                data = await _service.GetAllAsync(ct);
            }
        
            return new GetAllTodosResponse(data);
        }
        catch (Exception ex)
        {
            return new GetAllTodosResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

