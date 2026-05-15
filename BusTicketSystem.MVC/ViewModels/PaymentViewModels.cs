using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class PaymentRequestViewModel
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PaymentHistoryViewModel
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class RazorpayOrderViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }

    public class AgencyRevenueViewModel
    {
        public string AgencyName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalPayments { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public List<RevenueTrendPointViewModel> RevenueTrend { get; set; } = new();
        public List<TopRouteRevenueViewModel> TopRoutes { get; set; } = new();
    }

    public class RevenueTrendPointViewModel
    {
        public string Label { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class TopRouteRevenueViewModel
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public string DisplayName => !string.IsNullOrWhiteSpace(RouteName)
            ? RouteName
            : !string.IsNullOrWhiteSpace(FromCity) || !string.IsNullOrWhiteSpace(ToCity)
            ? $"{FromCity} -> {ToCity}"
            : "Route";
    }
}
