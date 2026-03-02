using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.API.Models;
using MeetingRoomBooking.API.Services;

namespace MeetingRoomBooking.API.Controllers;

/// <summary>
/// Controller for managing rooms.
/// Handles operations like retrieving rooms and updating room details.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Get all available rooms.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetRooms()
    {
        var rooms = await _roomService.GetRoomsAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Get a specific room by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetRoom(int id)
    {
        var room = await _roomService.GetRoomAsync(id);
        if (room == null)
            return NotFound(new { error = "Room not found" });

        return Ok(room);
    }

    /// <summary>
    /// Update room details (admin only).
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
    {
        var result = await _roomService.UpdateRoomAsync(id, request);
        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }
}
