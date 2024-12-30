namespace HotelBookingAPI.Dtos;

public class TravelerDetailDto
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BirthDate { get; set; }
    public bool IsActive { get; set; }
    public string? NationalId { get; set; }
    public string? RegistrationId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyContactName { get; set; }
    public bool HasEspecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }
    public string? CreatedOn { get; set; }
    public string? EditedBy { get; set; }
    public string? EditedOn { get; set; }
}
