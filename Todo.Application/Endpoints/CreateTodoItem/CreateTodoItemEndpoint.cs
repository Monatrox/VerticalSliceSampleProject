using System.Net;
using FastEndpoints;
using Todo.Infrastructure.Services;
using Todo.Domain.Objects;

namespace Todo.Application.Endpoints.CreateTodoItem;

public class CreateTodoItemEndpoint : Endpoint<CreateTodoItemRequest, CreateTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public CreateTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

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

            var data = await _service.CreateAsync(todoItem, ct);
            
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

