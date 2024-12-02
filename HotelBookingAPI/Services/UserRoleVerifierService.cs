using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingAPI.Services;

public class UserRoleVerifierService: IUserVerifierService
{
    private readonly UserManager<AppUser> _userManager;

    public UserRoleVerifierService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<bool> VerifyUserEmployeeOrAdminOrNull(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user is null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        if(roles.Contains("Admin") || roles.Contains("Employee"))
            return true;

        return false;
    }
}
