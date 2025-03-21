using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class CheckBookingService: ICheckBooking
{
    private readonly AppDbContext _dbContext;

    public CheckBookingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<BookingDuplicatedRS?> CheckDuplicateBooking(Booking booking)
    {
        var bookingDuplicated = await _dbContext.Bookings.FirstOrDefaultAsync(bt => bt.TravelerId == booking.TravelerId && bt.RoomId == booking.RoomId && bt.CheckInDate == booking.CheckInDate && bt.CheckOutDate == booking.CheckOutDate);
        if (bookingDuplicated != null)
        {
            var bookingDuplicatedError = new BookingDuplicatedRS { BookingDuplicatedId = bookingDuplicated.Id, Message = $"Não foi possível criar a reserva, pois já existe uma reserva duplicadam. Voucher da reserva existente: {bookingDuplicated.Id}." };
            return bookingDuplicatedError;
        }

        return null;

    }
}
