using HotelBookingAPI.Enums;

namespace HotelBookingAPI.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid TravelerId { get; set; }
    public Traveler Traveler { get; set; } = null!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }

    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EditedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
}
