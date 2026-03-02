namespace MeetingRoomBooking.API.Models;

/// <summary>
/// Represents a user in the system (employee or admin).
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Employee" or "Admin"
    
    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
