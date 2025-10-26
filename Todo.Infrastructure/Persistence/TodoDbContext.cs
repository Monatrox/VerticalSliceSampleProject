using Microsoft.EntityFrameworkCore;
using Todo.Domain.Objects;

namespace Todo.Infrastructure.Persistence;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }
    
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure TodoItem entity
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.ToTable("TodoItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsCompleted).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.HasIndex(e => e.IsCompleted);
            entity.HasIndex(e => e.DueDate);
        });

        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc);
        
        var todoItems = new List<TodoItem>
        {
            new TodoItem
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Complete project documentation",
                Description = "Write comprehensive documentation for the vertical slice architecture",
                IsCompleted = false,
                CreatedAt = now.AddDays(-5),
                DueDate = now.AddDays(2),
                Priority = 3 // High
            },
            new TodoItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Review pull requests",
                Description = "Review and merge pending pull requests from team members",
                IsCompleted = false,
                CreatedAt = now.AddDays(-3),
                DueDate = now.AddDays(1),
                Priority = 2 // Medium
            },
            new TodoItem
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Setup CI/CD pipeline",
                Description = "Configure automated deployment pipeline",
                IsCompleted = true,
                CreatedAt = now.AddDays(-10),
                CompletedAt = now.AddDays(-2),
                DueDate = now.AddDays(-3),
                Priority = 3 // High
            },
            new TodoItem
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Update dependencies",
                Description = "Update NuGet packages to latest stable versions",
                IsCompleted = false,
                CreatedAt = now.AddDays(-7),
                DueDate = now.AddDays(5),
                Priority = 1 // Low
            },
            new TodoItem
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Title = "Write unit tests",
                Description = "Add unit tests for new features",
                IsCompleted = false,
                CreatedAt = now.AddDays(-4),
                DueDate = now.AddDays(3),
                Priority = 2 // Medium
            }
        };

        modelBuilder.Entity<TodoItem>().HasData(todoItems);
    }
}

