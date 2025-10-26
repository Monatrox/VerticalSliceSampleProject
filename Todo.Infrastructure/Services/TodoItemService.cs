using Microsoft.EntityFrameworkCore;
using Todo.Domain.Objects;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Services;

public interface ITodoItemService
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetCompletedAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default);
    Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<TodoItem?> UpdateAsync(Guid id, TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<TodoItem?> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class TodoItemService : ITodoItemService
{
    private readonly TodoDbContext _context;

    public TodoItemService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Where(t => !t.IsCompleted)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Where(t => t.Priority == priority && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        // Validate priority
        if (todoItem.Priority < 1 || todoItem.Priority > 3)
        {
            throw new InvalidOperationException("Priority must be between 1 (Low) and 3 (High).");
        }

        // Set created date if not set
        if (todoItem.CreatedAt == default)
        {
            todoItem.CreatedAt = DateTime.UtcNow;
        }

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        
        return todoItem;
    }

    public async Task<TodoItem?> UpdateAsync(Guid id, TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var existing = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (existing == null)
        {
            return null;
        }

        // Validate priority
        if (todoItem.Priority < 1 || todoItem.Priority > 3)
        {
            throw new InvalidOperationException("Priority must be between 1 (Low) and 3 (High).");
        }

        existing.Title = todoItem.Title;
        existing.Description = todoItem.Description;
        existing.Priority = todoItem.Priority;
        existing.DueDate = todoItem.DueDate;
        existing.IsCompleted = todoItem.IsCompleted;
        
        // Update CompletedAt if status changed
        if (todoItem.IsCompleted && !existing.IsCompleted)
        {
            existing.CompletedAt = DateTime.UtcNow;
        }
        else if (!todoItem.IsCompleted && existing.IsCompleted)
        {
            existing.CompletedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        return existing;
    }

    public async Task<TodoItem?> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (todoItem == null)
        {
            return null;
        }

        if (todoItem.IsCompleted)
        {
            throw new InvalidOperationException("Todo item is already completed.");
        }

        todoItem.IsCompleted = true;
        todoItem.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return todoItem;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (todoItem == null)
        {
            return false;
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}

