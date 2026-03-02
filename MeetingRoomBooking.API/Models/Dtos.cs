namespace MeetingRoomBooking.API.Models;

/// <summary>
/// DTO for returning booking data in API responses.
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model for creating a new booking.
/// </summary>
public class CreateBookingRequest
{
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for returning room data in API responses.
/// </summary>
public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Building { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating a room.
/// </summary>
public class UpdateRoomRequest
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? Building { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for returning user data in API responses.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
