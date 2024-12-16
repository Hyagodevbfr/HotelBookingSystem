using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IUserVerifier
{
    Task<bool> VerifyUserEmployeeOrAdminOrNull(string id);
    Task<ServiceResultDto<AppUser>> IsDuplicateUser(object userDto);
    ServiceResultDto<AppUser> ValidateUser(object userDto);
    bool ValidateCpf(string cpf);
    bool ValidateRg(string rg);
}
