namespace HotelBookingAPI.Dtos;

public class UpdateTravelerDto
{
    //public string UserId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }
}