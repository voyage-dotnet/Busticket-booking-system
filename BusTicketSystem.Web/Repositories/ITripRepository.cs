using System;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories;

public interface ITripRepository
{
   Task<IEnumerable<Trip>> GetAllAsync();
   Task<Trip?> GetByIdAsync(int id);
   Task<IEnumerable<Trip>> SearchTripsAsync(string? from, string? to, DateTime? date);
   Task<IEnumerable<Trip>> GetTripsByAgencyAsync(int agencyId);
   Task<IEnumerable<Booking>> GetBookingsByTripIdAsync(int tripId);
   Task<Bus?> GetBusByIdAsync(int busId);
   Task<Trip> AddAsync(Trip trip);
   Task<Trip> UpdateAsync(Trip trip);
   Task<bool> ExistsAsync(int id);
   Task<Address?> GetAddressByIdAsync(int addressId);
}
