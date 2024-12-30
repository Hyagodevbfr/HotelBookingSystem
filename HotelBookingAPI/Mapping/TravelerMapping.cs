using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Mapping;

public class TravelerMapping : Profile
{
    public TravelerMapping()
    {
        CreateMap<Traveler,TravelerDetailDto>( );
    }
}
