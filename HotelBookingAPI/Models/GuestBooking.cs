namespace HotelBookingAPI.Models;

public class GuestBooking
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
}
