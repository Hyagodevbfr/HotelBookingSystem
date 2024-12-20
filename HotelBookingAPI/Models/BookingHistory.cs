namespace HotelBookingAPI.Models;

public class BookingHistory
{
    public Guid Id { get; set; }
    public string TravelerId { get; set; } = string.Empty;
    public Traveler? Traveler { get; set; }
    public List<Booking>? Bookings { get; set; }
}
