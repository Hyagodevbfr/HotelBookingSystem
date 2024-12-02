using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser,UserDetailDto>( );           
        CreateMap<AppUser, UserRegisterDto>( );
        CreateMap<ServiceResultDto<UserDetailDto>,UserDetailDto>( );
        CreateMap<UserDetailDto,AppUser>( );
    }
}
