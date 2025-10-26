using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Todo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "Description", "DueDate", "IsCompleted", "Priority", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Write comprehensive documentation for the vertical slice architecture", new DateTime(2025, 10, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, "Complete project documentation" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Review and merge pending pull requests from team members", new DateTime(2025, 10, 26, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, "Review pull requests" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Configure automated deployment pipeline", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Setup CI/CD pipeline" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Update NuGet packages to latest stable versions", new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, "Update dependencies" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Add unit tests for new features", new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, "Write unit tests" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_DueDate",
                table: "TodoItems",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_IsCompleted",
                table: "TodoItems",
                column: "IsCompleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");
        }
    }
}
