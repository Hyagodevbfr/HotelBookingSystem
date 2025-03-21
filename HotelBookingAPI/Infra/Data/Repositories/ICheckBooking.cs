using HotelBookingAPI.Dtos;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface ICheckBooking
{
    Task<BookingDuplicatedRS?> CheckDuplicateBooking(Booking booking);
}
