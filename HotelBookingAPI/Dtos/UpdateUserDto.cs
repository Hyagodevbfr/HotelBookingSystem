using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string RegistrationId { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
}
