using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IRoom
{
    Task<ServiceResultDto<RoomDto>> CreateRoom(RoomDto room, string userId);
    Task<ServiceResultDto<RoomDto>> EditRoom(RoomDto room,string userId, int roomId);
    Task<ServiceResultDto<RoomDetailDto>> GetRoom(int roomId);
    Task<ServiceResultDto<IEnumerable<RoomDetailDto>>> GetAllRooms();

    //Search
    Task<ServiceResultDto<IEnumerable<RoomSearchResponse>>> GetAvaliableRooms([FromBody]RoomSearchRequest searchRequest);
    Task<ServiceResultDto<DetailsAvailableRoomDto>> GetDetailsAvailableRoom(int id, string userId,[FromBody] RoomSearchRequest searchRequest);
}
