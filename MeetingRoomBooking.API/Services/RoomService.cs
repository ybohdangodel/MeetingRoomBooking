using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.API.Data;
using MeetingRoomBooking.API.Models;

namespace MeetingRoomBooking.API.Services;

/// <summary>
/// Interface for room operations.
/// </summary>
public interface IRoomService
{
    Task<RoomDto?> GetRoomAsync(int id);
    Task<List<RoomDto>> GetRoomsAsync();
    Task<OperationResult<RoomDto>> UpdateRoomAsync(int id, UpdateRoomRequest request);
}

/// <summary>
/// Service for managing rooms.
/// </summary>
public class RoomService : IRoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoomDto?> GetRoomAsync(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<List<RoomDto>> GetRoomsAsync()
    {
        var rooms = await _context.Rooms.ToListAsync();
        return rooms.Select(MapToDto).ToList();
    }

    public async Task<OperationResult<RoomDto>> UpdateRoomAsync(int id, UpdateRoomRequest request)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null)
            return OperationResult<RoomDto>.FailureResult("Room not found.");

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.Name))
            room.Name = request.Name;

        if (request.Capacity.HasValue && request.Capacity.Value > 0)
            room.Capacity = request.Capacity.Value;

        if (!string.IsNullOrEmpty(request.Building))
            room.Building = request.Building;

        if (!string.IsNullOrEmpty(request.Description))
            room.Description = request.Description;

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();

        return OperationResult<RoomDto>.SuccessResult(MapToDto(room));
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Building = room.Building,
            Description = room.Description
        };
    }
}
