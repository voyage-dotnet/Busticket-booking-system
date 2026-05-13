using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels;

// ── Page model: passed into Pay.cshtml ────────────────────────────────────────
public class PayViewModel
{
    // Read-only booking info (pre-filled from booking)
    public int    BookingId     { get; set; }
    public string RouteName     { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int    SeatNumber    { get; set; }
    public decimal Fare         { get; set; }

    // Derived fare breakdown
    public decimal BaseFare        => Fare - 7.50m;
    public decimal SeatSelectionFee => 5.50m;
    public decimal BookingFee       => 2.00m;
    public decimal TotalAmount      => Fare;

    // Which tab the user picked: "CARD" | "UPI"
    public string SelectedMethod { get; set; } = "CARD";

    // Card fields
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

// ── What the API returns after a successful card payment ─────────────────────
public class CardPaymentResultDto
{
    public string Type      { get; set; } = string.Empty;
    public string Message   { get; set; } = string.Empty;
    public int    PaymentId { get; set; }
    public string Status    { get; set; } = string.Empty;
}

// ── What the API returns for a UPI payment (QR code flow) ────────────────────
public class UpiPaymentResultDto
{
    public string Type      { get; set; } = string.Empty;
    public string Message   { get; set; } = string.Empty;
    public int    PaymentId { get; set; }
    public string QRCode    { get; set; } = string.Empty;   // base64 PNG
    public string UpiUrl    { get; set; } = string.Empty;
}

// ── Success page model ────────────────────────────────────────────────────────
public class PaymentSuccessViewModel
{
    public int     BookingId     { get; set; }
    public int     PaymentId     { get; set; }
    public string  RouteName     { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int     SeatNumber    { get; set; }
    public decimal Amount        { get; set; }
    public string  Method        { get; set; } = string.Empty;   // "CARD" | "UPI"
    public string  ConfirmationCode => $"VYG-{BookingId:D4}-XC";
}

// Matches PaymentResponseDTO from the API
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