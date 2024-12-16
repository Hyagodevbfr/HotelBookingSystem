namespace HotelBookingAPI.Dtos;

public class RoomDetailDto
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? EditedBy { get; set; }
    public DateTime? EditedOn { get; set; }
    public string? Type { get; set; } 
    public string? RoomName { get; set; }
    public int RoomsQuantity { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildCapacity { get; set; }
    public double PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public bool HasAirConditioning { get; set; } 
    public bool HasWiFi { get; set; }
    public bool HasTV { get; set; }
    public bool IsAccessible { get; set; }
    public List<string>? Amenities { get; set; }
}
