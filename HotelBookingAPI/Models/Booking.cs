using HotelBookingAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class Booking
{
    [Key]
    public int Id { get; set; }
    public string TravelerId { get; set; } = string.Empty;
    public Traveler? Traveler { get; set; }
    public List<GuestBooking>? GuestBookings { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EditedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
}
