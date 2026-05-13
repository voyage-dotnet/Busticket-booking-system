using BusTicketSystem.Web.ResponseWrapper;
namespace BusTicketSystem.Web.ResponseWrapper;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse()
    {
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Request Sucess", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> FailureResponse(string message, List<string>? error = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = error ?? new List<string>(),
            Data = default,
            StatusCode = statusCode
        };
    }
}

