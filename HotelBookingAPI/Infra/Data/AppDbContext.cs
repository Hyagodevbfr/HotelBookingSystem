﻿using Flunt.Notifications;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HotelBookingAPI.Infra.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Room>? Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingHistory> BookingHistories { get; set; }
    public DbSet<Traveler>? Travelers { get; set; }
    public DbSet<Guest>? Guests { get; set; }
    public DbSet<GuestBooking>? GuestBookings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Ignore<Notification>( );

        builder.Entity<AppUser>( )
        .HasIndex(u => u.NationalId)
        .IsUnique( );

        builder.Entity<AppUser>( )
            .HasIndex(u => u.RegistrationId)
            .IsUnique( );

        builder.Entity<Traveler>( )
            .HasOne(t => t.User)
            .WithOne( )
            .HasForeignKey<Traveler>(t => t.UserId);

        builder.Entity<Traveler>()
            .HasOne(t => t.BookingHistory)
            .WithOne( )
            .HasForeignKey<BookingHistory>(bh => bh.TravelerId);

        builder.Entity<BookingHistory>( )
            .HasOne(bh => bh.Traveler)
            .WithOne()
            .HasForeignKey<BookingHistory>(bh => bh.TravelerId);

        builder.Entity<Booking>( )
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId);

        builder.Entity<Booking>( )
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Booking>( )
            .HasOne(b => b.Traveler)
            .WithMany( )
            .HasForeignKey(b => b.TravelerId);

        builder.Entity<Booking>( )
            .HasKey(b => b.Id);

        builder.Entity<Booking>( )
            .Property(b => b.Id)
            .ValueGeneratedOnAdd( );


        builder.Entity<GuestBooking>( )
            .HasKey(gb => new { gb.GuestId,gb.BookingId });

        builder.Entity<GuestBooking>( )
            .HasOne(gb => gb.Guest)
            .WithMany(g => g.GuestBookings)
            .HasForeignKey(gb => gb.GuestId);

        builder.Entity<GuestBooking>( )
            .HasOne(gb => gb.Booking)
            .WithMany(b => b.GuestBookings)
            .HasForeignKey(gb => gb.BookingId);
    }
}
