using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IAccount
{
    Task<ServiceResultDto<AppUser>> Register(UserRegisterDto userRegisterDto);
    Task<ServiceResultDto<AppUser>> Login(UserLoginDto userLoginDto);
    Task<ServiceResultDto<UserDetailDto>> GetUserDetail(AppUser user);
    Task<ServiceResultDto<IEnumerable<UserDetailDto>>> GetUsers();

}
