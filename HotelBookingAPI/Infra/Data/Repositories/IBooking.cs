using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IBooking
{
    Task<ServiceResultDto<CreateBookingDto>> CreateBooking(BookingRequest bookingRequest);
}
