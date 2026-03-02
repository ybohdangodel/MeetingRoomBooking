using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.API.Models;

namespace MeetingRoomBooking.API.Data;

/// <summary>
/// Entity Framework Core DbContext for the application.
/// Manages database connections, entities, and configurations.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DateTime to always be UTC for PostgreSQL
        var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        // Configure Room entity
        modelBuilder.Entity<Room>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Room>()
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Room>()
            .Property(r => r.Building)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Room>()
            .Property(r => r.Capacity)
            .IsRequired();

        // Configure Device entity
        modelBuilder.Entity<Device>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Device>()
            .Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Device>()
            .Property(d => d.Type)
            .IsRequired()
            .HasMaxLength(100);

        // Configure Booking entity
        modelBuilder.Entity<Booking>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(50);

        // Configure foreign key relationships
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for common queries
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.UserId);

        // Seed initial data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "John Employee", Role = "Employee" },
            new User { Id = 2, Name = "Jane Admin", Role = "Admin" },
            new User { Id = 3, Name = "Bob Employee", Role = "Employee" }
        );

        // Seed rooms
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, Name = "Conference Room A", Capacity = 10, Building = "Main", Description = "Large conference room" },
            new Room { Id = 2, Name = "Meeting Room B", Capacity = 6, Building = "Main", Description = "Small meeting room" },
            new Room { Id = 3, Name = "Board Room", Capacity = 20, Building = "Executive", Description = "Executive board room" }
        );

        // Seed devices
        modelBuilder.Entity<Device>().HasData(
            new Device { Id = 1, Name = "Projector 1", Type = "Projector", IsAvailable = true },
            new Device { Id = 2, Name = "Whiteboard 1", Type = "Whiteboard", IsAvailable = true },
            new Device { Id = 3, Name = "Video Conference System", Type = "Camera", IsAvailable = true }
        );
    }
}
