using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class UserRegisterDto
{
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string NationalId { get; set; } = string.Empty;
    public string RegistrationId { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public DateTime CreatedOn { get; set; }
    public string? EditedBy { get; set; }
    public DateTime? EditedOn { get; set; }


}
