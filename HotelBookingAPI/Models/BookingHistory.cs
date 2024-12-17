namespace HotelBookingAPI.Models;

public class BookingHistory
{
    public Guid Id { get; set; }
    public string TravelerId { get; set; } = null!;
    public Traveler? Traveler { get; set; }
    public List<Booking>? Bookings { get; set; }
}
