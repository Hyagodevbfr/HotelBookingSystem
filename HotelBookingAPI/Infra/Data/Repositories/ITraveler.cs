using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface ITraveler
{
    Task<ServiceResultDto<CreateTravelerDto>> CreateTraveler(CreateTravelerDto createTravelerDto, string userId);
    Task<ServiceResultDto<UpdateTravelerDto>> UpdateTraveler(UpdateTravelerDto updateTravelerDto, string userId);
}
