using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.API.Data;

namespace MeetingRoomBooking.API.Services;

/// <summary>
/// Interface for detecting scheduling conflicts.
/// </summary>
public interface IConflictDetector
{
    /// <summary>
    /// Check if a room has any bookings that conflict with the given time range.
    /// </summary>
    Task<bool> HasConflictAsync(int roomId, DateTime startTime, DateTime endTime);
}

/// <summary>
/// Implementation of conflict detection for bookings.
/// </summary>
public class ConflictDetector : IConflictDetector
{
    private readonly AppDbContext _context;

    public ConflictDetector(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasConflictAsync(int roomId, DateTime startTime, DateTime endTime)
    {
        // Check if there are any confirmed bookings that overlap with the requested time slot
        var conflict = await _context.Bookings
            .Where(b => b.RoomId == roomId && b.Status != "Canceled")
            .Where(b => b.StartTime < endTime && b.EndTime > startTime)
            .AnyAsync();

        return conflict;
    }
}
