namespace BusTicketSystem.MVC.ViewModels;


public sealed class ApiEnvelopeDto<T>
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}