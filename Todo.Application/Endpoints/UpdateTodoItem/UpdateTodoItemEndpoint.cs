using System.Net;
using FastEndpoints;
using Todo.Infrastructure.Services;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.UpdateTodoItem;

public class UpdateTodoItemEndpoint : Endpoint<UpdateTodoItemRequest, UpdateTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public UpdateTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

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

            var data = await _service.UpdateAsync(request.Id, todoItem, ct);
            
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

