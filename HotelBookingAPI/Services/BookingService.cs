using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;

namespace HotelBookingAPI.Services;

public class BookingService: IBooking
{
    public Task<ServiceResultDto<CreateBookingDto>> CreateBooking(CreateBookingDto createBookingDto)
    {
        throw new NotImplementedException( );
    }
}
