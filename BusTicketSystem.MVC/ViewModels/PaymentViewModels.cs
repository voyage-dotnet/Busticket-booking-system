using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels;


public class PayViewModel
{
    
    public int    BookingId     { get; set; }
    public string RouteName     { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int    SeatNumber    { get; set; }
    public decimal Fare         { get; set; }

    
    public decimal BaseFare        => Fare - 7.50m;
    public decimal SeatSelectionFee => 5.50m;
    public decimal BookingFee       => 2.00m;
    public decimal TotalAmount      => Fare;

    
    public string SelectedMethod { get; set; } = "CARD";

    
    [Display(Name = "Cardholder Name")]
    public string? CardHolderName { get; set; }

    [Display(Name = "Card Number")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must be 16 digits.")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiry (MM/YY)")]
    public string? Expiry { get; set; }

    [Display(Name = "CVV")]
    [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3–4 digits.")]
    public string? CVV { get; set; }
}


public class CardPaymentResultDto
{
    public string Type      { get; set; } = string.Empty;
    public string Message   { get; set; } = string.Empty;
    public int    PaymentId { get; set; }
    public string Status    { get; set; } = string.Empty;
}


public class UpiPaymentResultDto
{
    public string Type      { get; set; } = string.Empty;
    public string Message   { get; set; } = string.Empty;
    public int    PaymentId { get; set; }
    public string QRCode    { get; set; } = string.Empty;   
    public string UpiUrl    { get; set; } = string.Empty;
}


public class PaymentSuccessViewModel
{
    public int     BookingId     { get; set; }
    public int     PaymentId     { get; set; }
    public string  RouteName     { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int     SeatNumber    { get; set; }
    public decimal Amount        { get; set; }
    public string  Method        { get; set; } = string.Empty;   
    public string  ConfirmationCode => $"VYG-{BookingId:D4}-XC";
}


public class PaymentResponseDto
{
    public int     PaymentId     { get; set; }
    public int     BookingId     { get; set; }
    public string  RouteName     { get; set; } = string.Empty;
    public decimal Amount        { get; set; }
    public string  PaymentStatus { get; set; } = string.Empty;
    public string? QRCode        { get; set; }
    public string? UpiUrl        { get; set; }
}