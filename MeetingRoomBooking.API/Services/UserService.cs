using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.API.Data;
using MeetingRoomBooking.API.Models;

namespace MeetingRoomBooking.API.Services;

/// <summary>
/// Interface for user operations.
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetUserAsync(int id);
    Task<List<UserDto>> GetUsersAsync();
}

/// <summary>
/// Service for managing users.
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(MapToDto).ToList();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Role = user.Role
        };
    }
}
