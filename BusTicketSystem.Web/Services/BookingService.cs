using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingResponseDTO> CreateBookingAsync(BookingRequestDTO request)
    {
        // Check if seat is available
        var seat = await _bookingRepository
            .GetAvailableSeatAsync(request.TripId, request.SeatNumber);

        if (seat == null)
            throw new BadRequestException("Seat is not available");

        // Hold the seat
        seat.Status = "Pending";
        await _bookingRepository.UpdateAsync(seat);

        // Start tracking for 10 minute timeout
        BookingTimeoutHelper.TrackBooking(seat.BookingId);

        return BookingMapper.ToDto(seat);
    }

    public async Task<BookingResponseDTO> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        return BookingMapper.ToDto(booking);
    }

    public async Task<List<BookingResponseDTO>> GetMyBookingsAsync(int customerId)
    {
        var bookings = await _bookingRepository.GetByCustomerIdAsync(customerId);
        return BookingMapper.ToDtoList(bookings);
    }

    public async Task<List<BookingResponseDTO>> GetBookingsByTripAsync(int tripId)
    {
        var bookings = await _bookingRepository.GetByTripIdAsync(tripId);
        return BookingMapper.ToDtoList(bookings);
    }

    public async Task<List<int>> GetAvailableSeatsAsync(int tripId)
    {
        return await _bookingRepository.GetAvailableSeatNumbersAsync(tripId);
    }
}