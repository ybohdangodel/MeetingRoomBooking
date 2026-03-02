namespace MeetingRoomBooking.API.Models;

/// <summary>
/// Represents a meeting room that can be booked.
/// </summary>
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Building { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
