using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.Validator;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace BusTicketSystem.Web.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public PaymentService(
        IPaymentRepository repo,
        IHttpClientFactory httpClientFactory,
        IConfiguration config)
    {
        _repo = repo;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    // ════════════════════════════════════════════════════════════════════════
    //  POST /api/payments
    //  PaymentMethod used ONLY to decide flow — never assigned to Payment entity
    //  DB columns used: PaymentId, BookingId, CustomerId, Amount,
    //                   PaymentDate, PaymentStatus
    // ════════════════════════════════════════════════════════════════════════

    public async Task<object> ProcessPaymentAsync(int customerId, PaymentRequestDTO dto)
    {
        // 1. Validate
        var errors = PaymentRequestValidator.Validate(dto);
        if (errors.Any())
            throw new BadRequestException(string.Join(" | ", errors));

        // 2. Check booking exists
        var booking = await _repo.GetBookingByIdAsync(dto.BookingId)
            ?? throw new NotFoundException($"Booking {dto.BookingId}");

        // 3. Check not already paid
        var existing = await _repo.GetByBookingIdAsync(dto.BookingId);
        if (existing != null && existing.PaymentStatus == "Success")
            throw new BadRequestException("This booking has already been paid.");

        // 4. Check not cancelled
        if (booking.Status == "Cancelled")
            throw new BadRequestException("Cannot pay for a cancelled booking.");

        // 5. Validate amount matches trip fare
        if (dto.Amount != booking.Trip!.Fare)
            throw new BadRequestException(
                $"Amount must match the trip fare of {booking.Trip.Fare}.");

        // 6. Build Payment — only DB columns, NO PaymentMethod property
        //var nextId = await _repo.GetNextPaymentIdAsync();
        var payment = new Models.Payment
        {
            //PaymentId = nextId,
            BookingId = dto.BookingId,
            CustomerId = customerId,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentStatus = "Success"           // will update to "Success" for CARD below
        };

        await _repo.AddAsync(payment);

        // ── UPI: generate QR, status stays Pending until ConfirmUPI called ───
        if (dto.PaymentMethod!.Equals("UPI", StringComparison.OrdinalIgnoreCase))
        {
            string upiUrl =
                $"upi://pay?pa=busticket@upi&pn=BusTicketSystem&am={dto.Amount}&cu=INR&tn=Booking_{dto.BookingId}";

            string qrApiUrl =
                $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(upiUrl)}";

            byte[] qrBytes;
            try
            {
                var client = _httpClientFactory.CreateClient("default");
                qrBytes = await client.GetByteArrayAsync(qrApiUrl);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"QR generation failed: {ex.Message}");
            }

            return new
            {
                Type = "QR",
                Message = "Scan QR to Pay via UPI",
                PaymentId = payment.PaymentId,
                QRCode = Convert.ToBase64String(qrBytes),
                UpiUrl = upiUrl
            };
        }

        // ── CARD: validate and mark Success immediately ────────────────────────
        if (dto.PaymentMethod.Equals("CARD", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(dto.CardNumber) && dto.CardNumber.Length == 16)
            {
                payment.PaymentStatus = "Success";   // only DB field updated
                await _repo.UpdateAsync(payment);

                await _repo.UpdateBookingStatusAsync(dto.BookingId, "Confirmed");

                return new
                {
                    Type = "CARD",
                    Message = "Card Payment Successful",
                    PaymentId = payment.PaymentId,
                    Status = payment.PaymentStatus
                };
            }

            throw new PaymentFailedException("Invalid Card Details. Card number must be 16 digits.");
        }

        throw new BadRequestException("Invalid Payment Method. Use 'UPI' or 'CARD'.");
    }

    // ════════════════════════════════════════════════════════════════════════
    //  POST /api/payments/confirm/{bookingId} — Confirm UPI after QR scan
    // ════════════════════════════════════════════════════════════════════════

    public async Task<string> ConfirmUPIPaymentAsync(int bookingId)
    {
        var payment = await _repo.GetByBookingIdAsync(bookingId)
            ?? throw new NotFoundException($"Payment not found for booking {bookingId}");

        if (payment.PaymentStatus == "Success")
            throw new BadRequestException("Payment already confirmed.");

        payment.PaymentStatus = "Success";
        await _repo.UpdateAsync(payment);

        await _repo.UpdateBookingStatusAsync(bookingId, "Confirmed");

        return "UPI Payment Confirmed Successfully.";
    }

    // ════════════════════════════════════════════════════════════════════════
    //  RAZORPAY — Step 1: Create Order
    // ════════════════════════════════════════════════════════════════════════

    public async Task<object> CreateRazorpayOrderAsync(int customerId, CreateOrderDTO dto)
    {
        var keyId = _config["Razorpay:KeyId"]!;
        var keySecret = _config["Razorpay:KeySecret"]!;

        var client = new RazorpayClient(keyId, keySecret);
        var options = new Dictionary<string, object>
        {
            { "amount",          (int)(dto.Amount * 100) },
            { "currency",        dto.Currency },
            { "receipt",         $"booking_{dto.BookingId}_{DateTime.UtcNow.Ticks}" },
            { "payment_capture", 1 }
        };

        var order = client.Order.Create(options);
        string orderId = order["id"].ToString()!;

        // Save Pending payment — only DB columns, NO PaymentMethod
        //var nextId = await _repo.GetNextPaymentIdAsync();
        var payment = new Models.Payment
        {
            //PaymentId = nextId,
            BookingId = dto.BookingId,
            CustomerId = customerId,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentStatus = "Pending"
        };
        await _repo.AddAsync(payment);

        return new
        {
            OrderId = orderId,
            Currency = dto.Currency,
            Amount = (int)(dto.Amount * 100),
            KeyId = keyId,
            PaymentId = payment.PaymentId
        };
    }

    // ════════════════════════════════════════════════════════════════════════
    //  RAZORPAY — Step 2: Verify & Save
    // ════════════════════════════════════════════════════════════════════════

    public async Task<object> VerifyAndSavePaymentAsync(VerifyPaymentDTO dto)
    {
        var keySecret = _config["Razorpay:KeySecret"]!;

        string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSig = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

        if (computedSig != dto.RazorpaySignature)
            throw new PaymentFailedException("Payment signature verification failed.");

        var payment = await _repo.GetByBookingIdAsync(dto.BookingId)
            ?? throw new NotFoundException("Payment record not found.");

        payment.PaymentStatus = "Success";
        await _repo.UpdateAsync(payment);

        await _repo.UpdateBookingStatusAsync(dto.BookingId, "Confirmed");

        return new
        {
            Message = "Payment Verified Successfully",
            PaymentId = payment.PaymentId,
            Status = payment.PaymentStatus,
            RazorpayPaymentId = dto.RazorpayPaymentId
        };
    }

    // ════════════════════════════════════════════════════════════════════════
    //  GET /api/payments/{id}
    // ════════════════════════════════════════════════════════════════════════

    public async Task<PaymentResponseDTO?> GetPaymentByIdAsync(int customerId, int paymentId)
    {
        var payment = await _repo.GetByIdAsync(paymentId)
            ?? throw new NotFoundException($"Payment {paymentId}");

        if (payment.CustomerId != customerId)
            throw new ForbiddenException("You are not authorized to view this payment.");

        return PaymentMapper.ToPaymentResponseDTO(payment);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  GET /api/payments/booking/{bookingId}
    // ════════════════════════════════════════════════════════════════════════

    public async Task<PaymentResponseDTO?> GetPaymentByBookingIdAsync(int customerId, int bookingId)
    {
        var payment = await _repo.GetByBookingIdAsync(bookingId)
            ?? throw new NotFoundException($"Payment for booking {bookingId}");

        if (payment.CustomerId != customerId)
            throw new ForbiddenException("You are not authorized to view this payment.");

        return PaymentMapper.ToPaymentResponseDTO(payment);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  GET /api/payments/my
    // ════════════════════════════════════════════════════════════════════════

    public async Task<List<PaymentHistoryDTO>> GetMyPaymentHistoryAsync(int customerId)
    {
        var payments = await _repo.GetAllByCustomerIdAsync(customerId);
        return payments.Select(PaymentMapper.ToPaymentHistoryDTO).ToList();
    }

    // ════════════════════════════════════════════════════════════════════════
    //  GET /api/payments/agency/revenue
    // ════════════════════════════════════════════════════════════════════════

    public async Task<AgencyRevenueDTO> GetAgencyRevenueAsync(int agencyId)
    {
        var payments = await _repo.GetAllByAgencyAsync(agencyId);
        var agencyName = payments.FirstOrDefault()
                            ?.Booking?.Trip?.Bus?.Office?.Agency?.Name
                         ?? string.Empty;

        return new AgencyRevenueDTO
        {
            AgencyId = agencyId,
            AgencyName = agencyName,
            TotalRevenue = payments.Sum(p => p.Amount ?? 0m),
            TotalPayments = payments.Count
        };
    }

    // ════════════════════════════════════════════════════════════════════════
    //  GET /api/payments/agency/trip/{tripId}/revenue
    // ════════════════════════════════════════════════════════════════════════

    public async Task<TripRevenueDTO> GetTripRevenueAsync(int agencyId, int tripId)
    {
        var trip = await _repo.GetTripByIdAsync(tripId)
            ?? throw new NotFoundException($"Trip {tripId}");

        if (trip.Bus?.Office?.AgencyId != agencyId)
            throw new ForbiddenException("This trip does not belong to your agency.");

        var payments = await _repo.GetAllByTripAsync(tripId);

        return new TripRevenueDTO
        {
            TripId = tripId,
            RouteName = $"{trip.Route?.FromCity} -> {trip.Route?.ToCity}",
            DepartureTime = trip.DepartureTime,
            TotalPassengers = payments.Count,
            TotalRevenue = payments.Sum(p => p.Amount ?? 0m),
            Payments = payments.Select(PaymentMapper.ToPaymentHistoryDTO).ToList()
        };
    }
}