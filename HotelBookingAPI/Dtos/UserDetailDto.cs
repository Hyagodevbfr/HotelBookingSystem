using System.Reflection.Metadata.Ecma335;

namespace HotelBookingAPI.Dtos;

public class UserDetailDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
    public string? PhoneNumber { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public int AcessFailedCount { get; set; }
}
