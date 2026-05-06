public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse()
    {
        Errors = new List<string>();
    }

    // Success response
    public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    // Failure response
    public static ApiResponse<T> FailureResponse(string message, List<string> errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>(),
            StatusCode = statusCode,
            Data = default
        };
    }
}