using HotelBookingAPI.Enums;
using HotelBookingAPI.Models;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Dtos;

public class CreateBookingDto
{
    public string TravelerId { get; set; } = string.Empty;
    public List<GuestBookingDto>? GuestBookings { get; set; }
    public int RoomId { get; set; }
    public string CheckInDate { get; set; } = string.Empty;
    public string CheckOutDate { get; set; } = string.Empty;
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    [JsonIgnore]
    public decimal TotalPrice { get; set; }
    public string TotalPriceFormatted => TotalPrice.ToString("F2");
    public BookingStatus Status { get; set; }
}
