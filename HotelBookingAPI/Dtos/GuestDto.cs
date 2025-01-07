using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class GuestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalId { get; set; } = string.Empty;
    public string RegistrationId { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }
}
