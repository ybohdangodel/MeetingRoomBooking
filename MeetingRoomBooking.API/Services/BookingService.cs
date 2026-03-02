using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.API.Data;
using MeetingRoomBooking.API.Models;

namespace MeetingRoomBooking.API.Services;

/// <summary>
/// Interface for booking operations.
/// </summary>
public interface IBookingService
{
    Task<BookingDto?> GetBookingAsync(int id);
    Task<List<BookingDto>> GetBookingsAsync();
    Task<List<BookingDto>> GetUserBookingsAsync(int userId);
    Task<OperationResult<BookingDto>> CreateBookingAsync(CreateBookingRequest request);
    Task<OperationResult<BookingDto>> CancelBookingAsync(int bookingId);
    Task<OperationResult<BookingDto>> ApproveBookingAsync(int bookingId);
}

/// <summary>
/// Service for managing bookings.
/// Handles creation, cancellation, and conflict detection.
/// </summary>
public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly IConflictDetector _conflictDetector;

    public BookingService(AppDbContext context, IConflictDetector conflictDetector)
    {
        _context = context;
        _conflictDetector = conflictDetector;
    }

    public async Task<BookingDto?> GetBookingAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        return booking == null ? null : MapToDto(booking);
    }

    public async Task<List<BookingDto>> GetBookingsAsync()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .ToListAsync();

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingDto>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
            .ToListAsync();

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<OperationResult<BookingDto>> CreateBookingAsync(CreateBookingRequest request)
    {
        // Ensure incoming DateTimes are treated as UTC
        var startTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);
        var endTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc);

        // Validate request
        if (endTime <= startTime)
            return OperationResult<BookingDto>.FailureResult("End time must be after start time.");

        if (startTime < DateTime.UtcNow)
            return OperationResult<BookingDto>.FailureResult("Cannot book in the past.");

        // Verify room exists
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null)
            return OperationResult<BookingDto>.FailureResult("Room not found.");

        // Verify user exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
            return OperationResult<BookingDto>.FailureResult("User not found.");

        // Check for conflicts
        var hasConflict = await _conflictDetector.HasConflictAsync(
            request.RoomId, 
            startTime, 
            endTime
        );
        if (hasConflict)
            return OperationResult<BookingDto>.FailureResult("Room is already booked for this time slot.");

        // Create booking
        var booking = new Booking
        {
            UserId = request.UserId,
            RoomId = request.RoomId,
            StartTime = startTime,
            EndTime = endTime,
            Status = "Pending",
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return OperationResult<BookingDto>.SuccessResult(MapToDto(booking));
    }

    public async Task<OperationResult<BookingDto>> CancelBookingAsync(int bookingId)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null)
            return OperationResult<BookingDto>.FailureResult("Booking not found.");

        booking.Status = "Canceled";
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return OperationResult<BookingDto>.SuccessResult(MapToDto(booking));
    }

    public async Task<OperationResult<BookingDto>> ApproveBookingAsync(int bookingId)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null)
            return OperationResult<BookingDto>.FailureResult("Booking not found.");

        booking.Status = "Confirmed";
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return OperationResult<BookingDto>.SuccessResult(MapToDto(booking));
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            RoomId = booking.RoomId,
            RoomName = booking.Room?.Name ?? "Unknown",
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };
    }
}
