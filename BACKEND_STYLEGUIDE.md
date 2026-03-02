# Backend Style Guide (.NET 9)

## Overview
This guide establishes standards for the MeetingRoomBooking API. Follow these practices to ensure clean, maintainable, and testable code.

---

## Project Structure

```
MeetingRoomBooking/
├── MeetingRoomBooking.API/          # Main API project
│   ├── Controllers/                 # HTTP endpoints
│   ├── Services/                    # Business logic
│   ├── Models/                      # Domain entities + DTOs
│   ├── Data/                        # EF Core DbContext, migrations
│   ├── Middleware/                  # Auth, error handling
│   ├── Program.cs                   # Startup config
│   └── appsettings.json            # Environment config
├── MeetingRoomBooking.Tests/        # Unit & integration tests
│   ├── ServiceTests/
│   ├── ControllerTests/
│   └── Helpers/                     # Test fixtures, mocks
└── README.md
```

---

## Naming Conventions

### Classes & Properties
- **PascalCase** for all public classes, methods, properties.
- **Example**: `BookingService`, `Room`, `CreateBookingRequest`, `ConflictDetector`.

### Local Variables & Parameters
- **camelCase** for names in method bodies.
- **Example**: `var bookingId = 123; var roomList = await GetRooms();`

### Async Methods
- Suffix with `Async`: `GetBookingsAsync()`, `CreateBookingAsync()`, `UpdateRoomAsync()`.

### Constants
- **UPPER_SNAKE_CASE**.
- **Example**: `public const int MAX_ROOM_CAPACITY = 500;`

### Database Tables
- **Plural form in singular context** (EF maps singular C# entities to plural SQL table names).
- **Entity**: `public class Booking {}` → **Table**: `Bookings`.

---

## Code Style

### 1. Controllers
- **Responsibility**: Parse HTTP requests, route to services, return responses.
- **No business logic in controllers** — delegate to services.
- **One controller per resource** (RoomsController, BookingsController, UsersController).

```csharp
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bookingService.CreateBookingAsync(request);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetBooking), new { id = result.Data.Id }, result.Data);
    }
}
```

### 2. Services
- **Responsibility**: Implement business logic, call repositories, manage transactions.
- **Use dependency injection** — inject IRepository/DbContext.
- **Return result types** (either `OperationResult<T>` or throw custom exceptions for known errors).

```csharp
public interface IBookingService
{
    Task<BookingDto?> GetBookingAsync(int id);
    Task<OperationResult<BookingDto>> CreateBookingAsync(CreateBookingRequest request);
    Task<List<BookingDto>> GetUserBookingsAsync(int userId);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IConflictDetector _conflictDetector;

    public BookingService(IBookingRepository repository, IConflictDetector conflictDetector)
    {
        _repository = repository;
        _conflictDetector = conflictDetector;
    }

    public async Task<OperationResult<BookingDto>> CreateBookingAsync(CreateBookingRequest request)
    {
        // Validate request
        if (request.EndTime <= request.StartTime)
            return OperationResult<BookingDto>.Failure("End time must be after start time.");

        // Check for conflicts
        var hasConflict = await _conflictDetector.HasConflictAsync(request.RoomId, request.StartTime, request.EndTime);
        if (hasConflict)
            return OperationResult<BookingDto>.Failure("Room is already booked for this time slot.");

        // Create and save
        var booking = new Booking 
        { 
            UserId = request.UserId, 
            RoomId = request.RoomId, 
            StartTime = request.StartTime, 
            EndTime = request.EndTime,
            Status = BookingStatus.Pending
        };

        await _repository.AddAsync(booking);
        return OperationResult<BookingDto>.Success(MapToDto(booking));
    }
}
```

### 3. Models Layer
- **Entities**: Domain objects (User, Room, Booking) — only live in Data/Models.
- **DTOs**: Data transfer objects for API responses — separate from entities.
- **Requests**: Inbound API objects (CreateBookingRequest, UpdateRoomRequest).

```csharp
// Entity
public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}

// DTO (for API response)
public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
}

// Request (for API input)
public class CreateBookingRequest
{
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
```

### 4. Data Access (EF Core)

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Config handled in Program.cs dependency injection
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId);
    }
}
```

**Migrations**:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Async/Await
- **Always use async** for I/O-bound operations (database, HTTP).
- **Never use `.Result` or `.Wait()`** — use `await`.

```csharp
// ✅ Good
public async Task<List<Booking>> GetAllAsync()
{
    return await _context.Bookings.ToListAsync();
}

// ❌ Bad
public List<Booking> GetAll()
{
    return _context.Bookings.ToList().Result; // Blocks thread!
}
```

---

## Error Handling

### Result Type Pattern
Use a simple result wrapper for service operations:

```csharp
public class OperationResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static OperationResult<T> Success(T data) => 
        new() { Success = true, Data = data };

    public static OperationResult<T> Failure(string error) => 
        new() { Success = false, ErrorMessage = error };
}
```

### Exception Handling
- Catch expected exceptions in controllers/middleware.
- Log unexpected exceptions (use Console or ILogger in tests).

```csharp
try
{
    var booking = await _bookingService.GetBookingAsync(id);
    return Ok(booking);
}
catch (InvalidOperationException ex)
{
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex}");
    return StatusCode(500, "Internal server error");
}
```

---

## API Design

### HTTP Verbs
- **GET** `/api/rooms` — retrieve list.
- **GET** `/api/rooms/{id}` — retrieve single.
- **POST** `/api/rooms` — create new.
- **PUT** `/api/rooms/{id}` — update.
- **DELETE** `/api/rooms/{id}` — delete.

### Response Format
```json
{
  "id": 1,
  "name": "Conference Room A",
  "capacity": 10
}
```

### Error Response
```json
{
  "error": "Room not found"
}
```

---

## Comments & Documentation

### XML Documentation
Document **public methods and classes**:

```csharp
/// <summary>
/// Creates a new booking if no conflicts exist.
/// </summary>
/// <param name="request">The booking request details.</param>
/// <returns>Success with booking data, or failure with error message.</returns>
public async Task<OperationResult<BookingDto>> CreateBookingAsync(CreateBookingRequest request)
{
    // Implementation
}
```

### Code Comments
- Use sparingly; code should be self-explanatory.
- Comment *why*, not *what*.

```csharp
// ✅ Good
// Check conflicts first to avoid wasting DB transaction.
var hasConflict = await _conflictDetector.HasConflictAsync(...);

// ❌ Bad
// var hasConflict = true;  // This gets conflict status
```

---

## Testing

### Naming
- Test class: `[ServiceName]Tests`.
- Test method: `[MethodName]_[Scenario]_[ExpectedResult]`.

### Example
```csharp
[Fact]
public async Task CreateBookingAsync_WithNoConflict_ReturnsSuccess()
{
    // Arrange
    var service = new BookingService(_mockRepository.Object, _mockDetector.Object);
    var request = new CreateBookingRequest { /* ... */ };

    // Act
    var result = await service.CreateBookingAsync(request);

    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.Data);
}

[Fact]
public async Task CreateBookingAsync_WithConflict_ReturnsFailure()
{
    // Arrange
    var service = new BookingService(_mockRepository.Object, _mockDetector.Object);
    _mockDetector.Setup(d => d.HasConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
        .ReturnsAsync(true);

    // Act
    var result = await service.CreateBookingAsync(new CreateBookingRequest { /* ... */ });

    // Assert
    Assert.False(result.Success);
    Assert.Contains("conflict", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
}
```

---

## Dependency Injection (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IConflictDetector, ConflictDetector>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

---

## Summary Checklist
- ✅ PascalCase for classes/methods, camelCase for locals.
- ✅ Async all the way; no `.Result`.
- ✅ Controllers route & return; Services contain logic.
- ✅ Use DTOs; separate from entities.
- ✅ Result types for operation outcomes.
- ✅ Minimal, meaningful comments.
- ✅ XML docs on public members.
- ✅ Test naming: `[Method]_[Scenario]_[Expected]`.
