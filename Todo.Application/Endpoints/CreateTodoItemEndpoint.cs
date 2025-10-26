using System.Net;
using FastEndpoints;
using Todo.Domain.Contracts.Endpoints.CreateTodoItem;
using Todo.Infrastructure.Services;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints;

public class CreateTodoItemEndpoint(ITodoItemService service) : Endpoint<CreateTodoItemRequest, CreateTodoItemResponse>
{
    public override void Configure()
    {
        Post("/todos");
        AllowAnonymous();
    }

    public override async Task<CreateTodoItemResponse> ExecuteAsync(CreateTodoItemRequest request, CancellationToken ct)
    {
        try
        {
            var todoItem = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            var data = await service.CreateAsync(todoItem, ct);
            
            return new CreateTodoItemResponse(data);
        }
        catch (InvalidOperationException ex)
        {
            return new CreateTodoItemResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            return new CreateTodoItemResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

