using HotelBookingAPI.Enums;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Dtos;

public class CreateBookingDto
{
    public string TravelerId { get; set; } = string.Empty;
    public List<GuestBooking>? GuestBookings { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
}
