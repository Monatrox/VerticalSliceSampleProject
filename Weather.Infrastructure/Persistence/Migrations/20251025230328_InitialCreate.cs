using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weather.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureC = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherForecasts_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "State" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Seattle", "Washington" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Portland", "Oregon" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Denver", "Colorado" }
                });

            migrationBuilder.InsertData(
                table: "WeatherForecasts",
                columns: new[] { "Id", "Date", "LocationId", "Summary", "TemperatureC" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateOnly(2025, 10, 25), new Guid("11111111-1111-1111-1111-111111111111"), "Chilly", 8 },
                    { new Guid("44444444-4444-4444-4444-444444444445"), new DateOnly(2025, 10, 26), new Guid("11111111-1111-1111-1111-111111111111"), "Cool", 10 },
                    { new Guid("44444444-4444-4444-4444-444444444446"), new DateOnly(2025, 10, 27), new Guid("11111111-1111-1111-1111-111111111111"), "Mild", 12 },
                    { new Guid("44444444-4444-4444-4444-444444444447"), new DateOnly(2025, 10, 28), new Guid("11111111-1111-1111-1111-111111111111"), "Bracing", 9 },
                    { new Guid("44444444-4444-4444-4444-444444444448"), new DateOnly(2025, 10, 29), new Guid("11111111-1111-1111-1111-111111111111"), "Mild", 11 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateOnly(2025, 10, 25), new Guid("22222222-2222-2222-2222-222222222222"), "Cool", 7 },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new DateOnly(2025, 10, 26), new Guid("22222222-2222-2222-2222-222222222222"), "Mild", 9 },
                    { new Guid("55555555-5555-5555-5555-555555555557"), new DateOnly(2025, 10, 27), new Guid("22222222-2222-2222-2222-222222222222"), "Warm", 13 },
                    { new Guid("55555555-5555-5555-5555-555555555558"), new DateOnly(2025, 10, 28), new Guid("22222222-2222-2222-2222-222222222222"), "Mild", 10 },
                    { new Guid("55555555-5555-5555-5555-555555555559"), new DateOnly(2025, 10, 29), new Guid("22222222-2222-2222-2222-222222222222"), "Balmy", 14 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateOnly(2025, 10, 25), new Guid("33333333-3333-3333-3333-333333333333"), "Freezing", -2 },
                    { new Guid("66666666-6666-6666-6666-666666666667"), new DateOnly(2025, 10, 26), new Guid("33333333-3333-3333-3333-333333333333"), "Bracing", 0 },
                    { new Guid("66666666-6666-6666-6666-666666666668"), new DateOnly(2025, 10, 27), new Guid("33333333-3333-3333-3333-333333333333"), "Chilly", 5 },
                    { new Guid("66666666-6666-6666-6666-666666666669"), new DateOnly(2025, 10, 28), new Guid("33333333-3333-3333-3333-333333333333"), "Cool", 8 },
                    { new Guid("66666666-6666-6666-6666-666666666670"), new DateOnly(2025, 10, 29), new Guid("33333333-3333-3333-3333-333333333333"), "Mild", 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_City_State",
                table: "Locations",
                columns: new[] { "City", "State" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_LocationId",
                table: "WeatherForecasts",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherForecasts");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
