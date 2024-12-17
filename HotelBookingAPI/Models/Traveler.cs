using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class Traveler
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }

    public List<Booking>? Bookings { get; set; }
    public BookingHistory? BookingHistory { get; set; }
    public DateTime? LastReservationDate { get; set; }
}
