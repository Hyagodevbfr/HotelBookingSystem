namespace HotelBookingAPI.Dtos;

public class DetailsAvailableRoom
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? RoomName { get; set; }
    public string? Description { get; set; }
    public int Guests { get; set; }
    public double PricePerNight { get; set; }
    public int Nights { get; set; }
    public double TotalStay { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasWiFi { get; set; }
    public bool HasTV { get; set; }
    public bool IsAccessible { get; set; }
    public List<string>? Amenities { get; set; }
}
