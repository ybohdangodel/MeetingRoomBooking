namespace MeetingRoomBooking.API.Models;

/// <summary>
/// Generic result wrapper for service operations.
/// Used to return success/failure with data or error message.
/// </summary>
public class OperationResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Create a successful result with data.
    /// </summary>
    public static OperationResult<T> SuccessResult(T data) => 
        new() { Success = true, Data = data };

    /// <summary>
    /// Create a failure result with error message.
    /// </summary>
    public static OperationResult<T> FailureResult(string error) => 
        new() { Success = false, ErrorMessage = error };
}
