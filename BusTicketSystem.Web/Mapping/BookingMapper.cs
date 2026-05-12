using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping;

public static class BookingMapper
{
    public static BookingResponseDTO ToDto(Booking booking)
    {
        return new BookingResponseDTO
        {
            BookingId = booking.BookingId,
            TripId = booking.TripId ?? 0,
            RouteName = $"{booking.Trip?.Route?.FromCity} -> {booking.Trip?.Route?.ToCity}",
            DepartureTime = booking.Trip?.DepartureTime ?? DateTime.MinValue,
            Fare = booking.Trip?.Fare ?? 0m,
            SeatNumber = booking.SeatNumber,
            Status = booking.Status
        };
    }

    public static List<BookingResponseDTO> ToDtoList(List<Booking> bookings)
        => bookings.Select(ToDto).ToList();
}