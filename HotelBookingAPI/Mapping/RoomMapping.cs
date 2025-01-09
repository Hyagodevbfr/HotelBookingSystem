using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Mapping;

public class RoomMapping : Profile
{
    public RoomMapping()
    {
        CreateMap<Room,RoomDetailDto>( );
        CreateMap<Room,DetailsAvailableRoomDto>( );
    }
}
