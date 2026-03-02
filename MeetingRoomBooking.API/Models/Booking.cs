namespace MeetingRoomBooking.API.Models;

/// <summary>
/// Represents a booking of a room by a user for a specific time period.
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Confirmed", "Canceled"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}
