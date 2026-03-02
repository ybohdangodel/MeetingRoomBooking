using MeetingRoomBooking.API.Data;

namespace MeetingRoomBooking.API.Middleware;

/// <summary>
/// Custom middleware for simple user authentication.
/// Reads X-User-Id header and validates the user exists.
/// </summary>
public class SimpleAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Extract user ID from header
        var userIdHeader = context.Request.Headers["X-User-Id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(userIdHeader) && int.TryParse(userIdHeader, out var userId))
        {
            // Check if user exists
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                // Store user info in HttpContext.Items for use in controllers
                context.Items["UserId"] = userId;
                context.Items["UserRole"] = user.Role;
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension methods for middleware setup.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSimpleAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SimpleAuthMiddleware>();
    }
}
