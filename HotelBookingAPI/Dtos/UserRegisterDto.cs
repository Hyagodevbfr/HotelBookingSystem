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
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
