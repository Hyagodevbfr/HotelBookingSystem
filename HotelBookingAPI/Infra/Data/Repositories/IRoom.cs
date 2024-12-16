using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IRoom
{
    Task<ServiceResultDto<RoomDto>> CreateRoom(RoomDto room, string userId);
    Task<ServiceResultDto<RoomDto>> EditRoom(RoomDto room,string userId, int roomId);
    Task<ServiceResultDto<RoomDetailDto>> GetRoom(int roomId);
    Task<ServiceResultDto<IEnumerable<RoomDetailDto>>> GetAllRooms();
}
