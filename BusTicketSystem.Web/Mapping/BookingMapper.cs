using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping;

public static class BookingMapper
{
    public static BookingResponseDTO ToDto(Booking booking)
    {
        return new BookingResponseDTO
        {
            BookingId  = booking.BookingId,
            TripId     = booking.TripId ?? 0,
            SeatNumber = booking.SeatNumber,
            Status     = booking.Status
        };
    }

    public static List<BookingResponseDTO> ToDtoList(List<Booking> bookings)
    {
        return bookings.Select(ToDto).ToList();
    }
}