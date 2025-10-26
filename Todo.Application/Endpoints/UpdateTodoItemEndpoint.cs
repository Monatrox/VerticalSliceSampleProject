using System.Net;
using FastEndpoints;
using Todo.Domain.Contracts.Endpoints.UpdateTodoItem;
using Todo.Infrastructure.Services;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints;

public class UpdateTodoItemEndpoint(ITodoItemService service) : Endpoint<UpdateTodoItemRequest, UpdateTodoItemResponse>
{
    public override void Configure()
    {
        Put("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<UpdateTodoItemResponse> ExecuteAsync(UpdateTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var todoItem = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                IsCompleted = request.IsCompleted
            };

            var data = await service.UpdateAsync(request.Id, todoItem, ct);
            
            return data is null 
                ? new UpdateTodoItemResponse(HttpStatusCode.NotFound)
                : new UpdateTodoItemResponse(data);
        }
        catch (InvalidOperationException ex)
        {
            return new UpdateTodoItemResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            return new UpdateTodoItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

