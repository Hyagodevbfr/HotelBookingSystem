using Flunt.Notifications;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HotelBookingAPI.Infra.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Room>? Rooms { get; set; }

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
    }
}
