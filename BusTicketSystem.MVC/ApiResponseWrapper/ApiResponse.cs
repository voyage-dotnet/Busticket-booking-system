namespace BusTicketSystem.MVC.ApiResponseWrapper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public List<string>? Errors { get; set; }

        public List<string> GetErrorList() => Errors ?? new List<string>();
    }
}
