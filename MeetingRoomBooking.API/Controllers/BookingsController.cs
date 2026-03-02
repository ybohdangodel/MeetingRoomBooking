using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.API.Models;
using MeetingRoomBooking.API.Services;

namespace MeetingRoomBooking.API.Controllers;

/// <summary>
/// Controller for managing bookings.
/// Handles operations like creating, retrieving, canceling, and approving bookings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Get all bookings (admin only).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetBookingsAsync();
        return Ok(bookings);
    }

    /// <summary>
    /// Get a specific booking by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingAsync(id);
        if (booking == null)
            return NotFound(new { error = "Booking not found" });

        return Ok(booking);
    }

    /// <summary>
    /// Get bookings for a specific user.
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<BookingDto>>> GetUserBookings(int userId)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    /// <summary>
    /// Create a new booking.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bookingService.CreateBookingAsync(request);
        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return CreatedAtAction(nameof(GetBooking), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Cancel an existing booking.
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<BookingDto>> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id);
        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Approve a booking (admin only).
    /// </summary>
    [HttpPut("{id}/approve")]
    public async Task<ActionResult<BookingDto>> ApproveBooking(int id)
    {
        var result = await _bookingService.ApproveBookingAsync(id);
        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }
}
