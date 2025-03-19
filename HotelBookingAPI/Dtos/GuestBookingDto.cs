using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class GuestBookingDto
{
    public int BookingId { get; set; }
    public int GuestId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string RegistrationId { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }

}
