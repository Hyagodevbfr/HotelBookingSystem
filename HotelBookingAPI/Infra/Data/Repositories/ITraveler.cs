using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface ITraveler
{
    Task<ServiceResultDto<CreateTravelerDto>> CreateTraveler(CreateTravelerDto createTravelerDto, string userId);
}
