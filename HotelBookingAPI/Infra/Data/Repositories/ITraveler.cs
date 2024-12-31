using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;
using System.Linq;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface ITraveler
{
    Task<ServiceResultDto<CreateTravelerDto>> CreateTraveler(CreateTravelerDto createTravelerDto, string userId);
    Task<ServiceResultDto<List<TravelerDetailDto>>> GetTravelers();
    Task<ServiceResultDto<TravelerDetailDto>> GetTravelerDetail(string travelerId);
    Task<ServiceResultDto<UpdateTravelerDto>> UpdateTraveler(UpdateTravelerDto updateTravelerDto, string userId, string authenticatedUser);
}
