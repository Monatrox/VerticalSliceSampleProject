using Microsoft.EntityFrameworkCore;
using Weather.Domain.Objects;

namespace Weather.Infrastructure.Persistence;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }
    
    public DbSet<Location> Locations { get; set; }
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Location entity
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.City, e.State }).IsUnique();
        });

        // Configure WeatherForecast entity
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.ToTable("WeatherForecasts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.TemperatureC).IsRequired();
            entity.Property(e => e.Summary).HasMaxLength(200);

            entity.HasOne(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        var locationIds = new
        {
            Seattle = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Portland = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Denver = Guid.Parse("33333333-3333-3333-3333-333333333333")
        };

        // Seed Locations
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = locationIds.Seattle, City = "Seattle", State = "Washington" },
            new Location { Id = locationIds.Portland, City = "Portland", State = "Oregon" },
            new Location { Id = locationIds.Denver, City = "Denver", State = "Colorado" }
        );

        // Seed WeatherForecasts with static, deterministic values
        var baseDate = new DateOnly(2025, 10, 25);

        var forecasts = new List<WeatherForecast>
        {
            // Seattle
            new WeatherForecast { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), LocationId = locationIds.Seattle, Date = baseDate.AddDays(0), TemperatureC = 8, Summary = "Chilly" },
            new WeatherForecast { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"), LocationId = locationIds.Seattle, Date = baseDate.AddDays(1), TemperatureC = 10, Summary = "Cool" },
            new WeatherForecast { Id = Guid.Parse("44444444-4444-4444-4444-444444444446"), LocationId = locationIds.Seattle, Date = baseDate.AddDays(2), TemperatureC = 12, Summary = "Mild" },
            new WeatherForecast { Id = Guid.Parse("44444444-4444-4444-4444-444444444447"), LocationId = locationIds.Seattle, Date = baseDate.AddDays(3), TemperatureC = 9, Summary = "Bracing" },
            new WeatherForecast { Id = Guid.Parse("44444444-4444-4444-4444-444444444448"), LocationId = locationIds.Seattle, Date = baseDate.AddDays(4), TemperatureC = 11, Summary = "Mild" },

            // Portland
            new WeatherForecast { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), LocationId = locationIds.Portland, Date = baseDate.AddDays(0), TemperatureC = 7, Summary = "Cool" },
            new WeatherForecast { Id = Guid.Parse("55555555-5555-5555-5555-555555555556"), LocationId = locationIds.Portland, Date = baseDate.AddDays(1), TemperatureC = 9, Summary = "Mild" },
            new WeatherForecast { Id = Guid.Parse("55555555-5555-5555-5555-555555555557"), LocationId = locationIds.Portland, Date = baseDate.AddDays(2), TemperatureC = 13, Summary = "Warm" },
            new WeatherForecast { Id = Guid.Parse("55555555-5555-5555-5555-555555555558"), LocationId = locationIds.Portland, Date = baseDate.AddDays(3), TemperatureC = 10, Summary = "Mild" },
            new WeatherForecast { Id = Guid.Parse("55555555-5555-5555-5555-555555555559"), LocationId = locationIds.Portland, Date = baseDate.AddDays(4), TemperatureC = 14, Summary = "Balmy" },

            // Denver
            new WeatherForecast { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), LocationId = locationIds.Denver, Date = baseDate.AddDays(0), TemperatureC = -2, Summary = "Freezing" },
            new WeatherForecast { Id = Guid.Parse("66666666-6666-6666-6666-666666666667"), LocationId = locationIds.Denver, Date = baseDate.AddDays(1), TemperatureC = 0, Summary = "Bracing" },
            new WeatherForecast { Id = Guid.Parse("66666666-6666-6666-6666-666666666668"), LocationId = locationIds.Denver, Date = baseDate.AddDays(2), TemperatureC = 5, Summary = "Chilly" },
            new WeatherForecast { Id = Guid.Parse("66666666-6666-6666-6666-666666666669"), LocationId = locationIds.Denver, Date = baseDate.AddDays(3), TemperatureC = 8, Summary = "Cool" },
            new WeatherForecast { Id = Guid.Parse("66666666-6666-6666-6666-666666666670"), LocationId = locationIds.Denver, Date = baseDate.AddDays(4), TemperatureC = 12, Summary = "Mild" }
        };

        modelBuilder.Entity<WeatherForecast>().HasData(forecasts);
    }
}