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
        CreateMap<UserRegisterDto,AppUser>( )
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));
        CreateMap<ServiceResultDto<UserDetailDto>,UserDetailDto>( );
        CreateMap<UserDetailDto,AppUser>( );
        CreateMap<UpdateUserDto, AppUser>( )
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EditedOn, opt => opt.MapFrom(src => DateTime.Now));
    }
}
