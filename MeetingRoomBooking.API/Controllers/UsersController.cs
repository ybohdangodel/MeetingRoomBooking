using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.API.Models;
using MeetingRoomBooking.API.Services;

namespace MeetingRoomBooking.API.Controllers;

/// <summary>
/// Controller for managing users.
/// Handles operations like retrieving user details.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get the current user (from X-User-Id header).
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdObj = HttpContext.Items["UserId"];
        if (userIdObj == null || !int.TryParse(userIdObj.ToString(), out var userId))
            return Unauthorized(new { error = "User not authenticated" });

        var user = await _userService.GetUserAsync(userId);
        if (user == null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }

    /// <summary>
    /// Get a specific user by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }

    /// <summary>
    /// Get all users (admin only).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
}
