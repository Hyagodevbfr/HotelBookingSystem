using AutoMapper;
using HotelBookingAPI.Dtos;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingAPI.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<IdentityRole,RoleResponseDto>( );
    }
}
