using System.Net;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Contracts.Endpoints.GetAllTodos;
using Todo.Infrastructure.Services;

namespace Todo.Application.Endpoints;

public class GetAllTodoItemsEndpoint(ITodoItemService service)
    : Endpoint<GetAllTodoItemsRequest, GetAllTodoItemsResponse>
{
    public override void Configure()
    {
        Get("/todos");
        AllowAnonymous();
        Options(x => x.CacheOutput(policy => policy
            .Expire(TimeSpan.FromMinutes(5)))
        );
    }

    public override async Task<GetAllTodoItemsResponse> ExecuteAsync(GetAllTodoItemsRequest request, CancellationToken ct)
    {
        try
        {
            IEnumerable<Domain.Objects.TodoItem> data;
            
            if (request.IsCompleted.HasValue)
            {
                data = request.IsCompleted.Value 
                    ? await service.GetCompletedAsync(ct)
                    : await service.GetActiveAsync(ct);
            }
            else
            {
                data = await service.GetAllAsync(ct);
            }
        
            return new GetAllTodoItemsResponse(data);
        }
        catch (Exception ex)
        {
            return new GetAllTodoItemsResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

