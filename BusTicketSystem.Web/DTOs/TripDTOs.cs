using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.DTOs;
public class TripSummaryDTO
{
    public int TripId { get; set; }
    public int RouteId { get; set; }
    public string FromCity { get; set; }
    public string ToCity { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Fare { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public string Status { get; set; }
}
public class TripDetailDTO : TripSummaryDTO
{
    public string? BreakPoints { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public BusMiniDTO Bus { get; set; }
    public DriverMiniDTO Driver { get; set; }
    public AgencyMiniDTO Agency { get; set; }
    public AddressMiniDTO BoardingAddress { get; set; }
}
public class TripSearchResultDTO
{
    public int TripId { get; set; }
    public string FromCity { get; set; }
    public string ToCity { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Fare { get; set; }
    public int AvailableSeats { get; set; }
    public string BusType { get; set; }
    public string AgencyName { get; set; }
    public string Status { get; set; }
}
public class SeatLayoutDTO
{
    public int TripId { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public List<SeatDTO> Seats { get; set; }
}

public class SeatDTO
{
    public int SeatNumber { get; set; }
    public string Status { get; set; }   
}
public class CreateTripRequestDTO
{
    public int RouteId { get; set; }
    public int BusId { get; set; }
    public int DriverId { get; set; }
    public int BoardingAddressId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Fare { get; set; }
}

public class UpdateTripRequestDTO
{
    public decimal? Fare { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int? AvailableSeats { get; set; }
    public string? Status { get; set; }
}
public class MyTripWithOccupancyDTO : TripSummaryDTO
{
    public int BookedSeats { get; set; }
    public double OccupancyPercentage { get; set; }
}

