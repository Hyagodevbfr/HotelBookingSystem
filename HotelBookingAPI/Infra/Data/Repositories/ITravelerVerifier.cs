using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface ITravelerVerifier
{
    Task<ServiceResultDto<Traveler>> IsDuplicateTraveler(object travelerDto, string travelId);
}
