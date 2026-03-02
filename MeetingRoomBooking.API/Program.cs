using MeetingRoomBooking.API.Data;
using MeetingRoomBooking.API.Middleware;
using MeetingRoomBooking.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from environment variable or use default PostgreSQL connection
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=meetingroom;Username=postgres;Password=postgres";

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register application services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConflictDetector, ConflictDetector>();

// Add controllers and OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Register middleware
app.UseSimpleAuth();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
