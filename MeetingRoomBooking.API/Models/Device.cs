namespace MeetingRoomBooking.API.Models;

/// <summary>
/// Represents a device (projector, whiteboard, etc.) that can be checked out.
/// </summary>
public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Projector", "Whiteboard", etc.
    public bool IsAvailable { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
}
