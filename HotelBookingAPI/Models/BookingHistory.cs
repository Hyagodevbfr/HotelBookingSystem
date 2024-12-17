namespace HotelBookingAPI.Models;

public class BookingHistory
{
    public Guid Id { get; set; }
    public Guid TravelerId { get; set; }
    public Traveler? Traveler { get; set; }
    public List<Booking>? Bookings { get; set; }
}
