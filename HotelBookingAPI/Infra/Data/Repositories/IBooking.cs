using HotelBookingAPI.Dtos;
using HotelBookingAPI.Enums;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IBooking
{
    Task<ServiceResultDto<CreateBookingDto>> CreateBooking(BookingRequest bookingRequest);
    Task<ServiceResultDto<List<BookingDto>>> GetAllBookings();
    Task<ServiceResultDto<BookingDto>> GetBooking(int id);
    Task<ServiceResultDto<string>> UpdateBookingStatus(int id, string userId,BookingStatus bookingStatus);
    Task<ServiceResultDto<List<BookingDto>>> GetBookingsByStatus(BookingStatus bookingStatus);
    Task<ServiceResultDto<BookingDto>> GetBookingByTravelerNationalId(string nationalId);

}
