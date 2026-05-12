namespace BusTicketSystem.Web.DTOs;

public class BookingRequestDTO
{
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
    //public int CustomerId { get; set; }
}

public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public int TripId { get; set; }
    public string RouteName { get; set; } = string.Empty;   // "Mumbai -> Pune"
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PaymentRequestDTO
{
    public int BookingId { get; set; }
    //public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }   // "UPI" | "CARD" — not stored in DB

    // Card fields (only when PaymentMethod = "CARD")
    public string? CardHolderName { get; set; }
    public string? CardNumber { get; set; }
    public string? Expiry { get; set; }
    public string? CVV { get; set; }
}

public class PaymentResponseDTO
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;  // "Pending" | "Success"

    // Only populated for UPI — generated at runtime, NOT from DB
    public string? QRCode { get; set; }
    public string? UpiUrl { get; set; }
}
// ─── RAZORPAY ─────────────────────────────────────────────────────────────────
public class CreateOrderDTO
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
}

public class VerifyPaymentDTO
{
    public int BookingId { get; set; }
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public string RazorpaySignature { get; set; } = string.Empty;
}
// ─── PAYMENT HISTORY ──────────────────────────────────────────────────────────
// Only DB columns — no PaymentMethod
public class PaymentHistoryDTO
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
}
// ─── AGENCY REVENUE ───────────────────────────────────────────────────────────
public class AgencyRevenueDTO
{
    public int AgencyId { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalPayments { get; set; }
}

public class TripRevenueDTO
{
    public int TripId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int TotalPassengers { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<PaymentHistoryDTO> Payments { get; set; } = new();
}