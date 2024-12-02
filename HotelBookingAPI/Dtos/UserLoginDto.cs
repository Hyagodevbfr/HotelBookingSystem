using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class UserLoginDto
{
    [EmailAddress, Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
