namespace BusTicketSystem.Web.Wrapper;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    // Static helper methods to create responses quickly
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data    = data
        };
    }

    public static ApiResponse<T> FailResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data    = default
        };
    }
}