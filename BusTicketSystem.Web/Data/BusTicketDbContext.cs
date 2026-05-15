using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Models;

public partial class BusTicketDbContext : DbContext
{
    public BusTicketDbContext()
    {
    }

    public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<AgencyOffice> AgencyOffices { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bus> Buses { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__addresse__CAA247C85AF46ADA");

            entity.ToTable("addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Address1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("state");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.AgencyId).HasName("PK__agencies__7224EBF8EFFEA418");

            entity.ToTable("agencies");

            entity.Property(e => e.AgencyId).HasColumnName("agency_id");
            entity.Property(e => e.ContactPersonName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("contact_person_name");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<AgencyOffice>(entity =>
        {
            entity.HasKey(e => e.OfficeId).HasName("PK__agency_o__2A196375DB5483C1");

            entity.ToTable("agency_offices");

            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.AgencyId).HasColumnName("agency_id");
            entity.Property(e => e.OfficeAddressId).HasColumnName("office_address_id");
            entity.Property(e => e.OfficeContactNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("office_contact_number");
            entity.Property(e => e.OfficeContactPersonName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("office_contact_person_name");
            entity.Property(e => e.OfficeMail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("office_mail");

            entity.HasOne(d => d.Agency).WithMany(p => p.AgencyOffices)
                .HasForeignKey(d => d.AgencyId)
                .HasConstraintName("FK_agency_offices_agency");

            entity.HasOne(d => d.OfficeAddress).WithMany(p => p.AgencyOffices)
                .HasForeignKey(d => d.OfficeAddressId)
                .HasConstraintName("FK_agency_offices_address");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__bookings__5DE3A5B10406B6AF");

            entity.ToTable("bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TripId).HasColumnName("trip_id");

            entity.HasOne(d => d.Trip).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_bookings_trip");
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__buses__6ACEF8EDDAED3518");

            entity.ToTable("buses");

            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("registration_number");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Office).WithMany(p => p.Buses)
                .HasForeignKey(d => d.OfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_buses_office");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85FDA639A8");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");

            entity.HasOne(d => d.Address).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_customers_address");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PK__drivers__A411C5BDB4EBA59D");

            entity.ToTable("drivers");

            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("license_number");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");

            entity.HasOne(d => d.Address).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_drivers_address");

            entity.HasOne(d => d.Office).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("FK_drivers_office");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payments__ED1FC9EA3D21A8D9");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("payment_status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_payments_booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_payments_customer");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__reviews__60883D9051252C0C");

            entity.ToTable("reviews");

            entity.Property(e => e.ReviewId)
                .ValueGeneratedNever()
                .HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasColumnType("datetime")
                .HasColumnName("review_date");
            entity.Property(e => e.TripId).HasColumnName("trip_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reviews_customer");

            entity.HasOne(d => d.Trip).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reviews_trip");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__routes__28F706FE9E02B7C9");

            entity.ToTable("routes");

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.BreakPoints).HasColumnName("break_points");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.FromCity)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("from_city");
            entity.Property(e => e.ToCity)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("to_city");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__trips__302A5D9EBB820751");

            entity.ToTable("trips");

            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("datetime")
                .HasColumnName("arrival_time");
            entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");
            entity.Property(e => e.BoardingAddressId).HasColumnName("boarding_address_id");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("datetime")
                .HasColumnName("departure_time");
            entity.Property(e => e.Driver1DriverId).HasColumnName("driver1_driver_id");
            entity.Property(e => e.Driver2DriverId).HasColumnName("driver2_driver_id");
            entity.Property(e => e.DroppingAddressId).HasColumnName("dropping_address_id");
            entity.Property(e => e.Fare)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("fare");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.TripDate)
                .HasColumnType("datetime")
                .HasColumnName("trip_date");

            entity.HasOne(d => d.Bus).WithMany(p => p.Trips)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trips_bus");

            entity.HasOne(d => d.Driver1Driver).WithMany(p => p.TripDriver1Drivers)
                .HasForeignKey(d => d.Driver1DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trips_driver1");

            entity.HasOne(d => d.Driver2Driver).WithMany(p => p.TripDriver2Drivers)
                .HasForeignKey(d => d.Driver2DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trips_driver2");

            entity.HasOne(d => d.Route).WithMany(p => p.Trips)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trips_route");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
