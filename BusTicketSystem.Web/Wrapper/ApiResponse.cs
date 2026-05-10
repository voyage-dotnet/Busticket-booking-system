namespace BusTicketSystem.Web.Wrapper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(
            T data,
            string message,
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> ErrorResponse(
            string message,
            int statusCode = 400,
            object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors
            };
        }

        // Used by your Agency code if you call FailureResponse(message, statusCode)
        public static ApiResponse<T> FailureResponse(
            string message,
            int statusCode = 400)
        {
            return ErrorResponse(message, statusCode);
        }

        // Used by AuthService: FailureResponse(message, errors, statusCode)
        public static ApiResponse<T> FailureResponse(
            string message,
            object? errors,
            int statusCode = 400)
        {
            return ErrorResponse(message, statusCode, errors);
        }
    }
}