using HotelBookingAPI.Dtos;
using HotelBookingAPI.Enums;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class BookingService: IBooking
{
    private readonly AppDbContext _dbContext;

    public BookingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ServiceResultDto<CreateBookingDto>> CreateBooking(BookingRequest bookingRequest)
    {
        var traveler = await _dbContext.Travelers!.FirstOrDefaultAsync(t => t.UserId == bookingRequest.TravelerId);
        if(traveler is null)
            return ServiceResultDto<CreateBookingDto>.Fail("Viajante não localizado.");

        var room = await _dbContext.Rooms!.FirstOrDefaultAsync(r => r.Id == bookingRequest.RoomId);
        if(room is null || room.RoomsQuantity <= 0)
            return ServiceResultDto<CreateBookingDto>.Fail("Quarto não disponível.");

        var booking = new Booking
        {
            TravelerId = bookingRequest.TravelerId,
            RoomId = bookingRequest.RoomId,
            CheckInDate = bookingRequest.CheckInDate,
            CheckOutDate = bookingRequest.CheckOutDate,
            TotalPrice = bookingRequest.TotalPrice,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            CreatedBy = bookingRequest.TravelerId,
            EditedBy = bookingRequest.TravelerId,
        };

        _dbContext.Bookings!.Add(booking);
        room.RoomsQuantity -= 1;

        if(bookingRequest.Guests != null)
        {
            foreach(var guest in bookingRequest.Guests)
            {
                var guestEntity = new Guest
                {
                    TravelerId = bookingRequest.TravelerId,
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    NationalId = guest.NationalId ?? string.Empty,
                    RegistrationId = guest.RegistrationId,
                    BirthDate = guest.BirthDate,
                    HasSpecialNeeds = guest.HasSpecialNeeds,
                    SpecialNeedsDetails = guest.SpecialNeedsDetails ?? string.Empty,
                    DietaryPreferences = guest.DietaryPreferences ?? string.Empty,
                };
                _dbContext.Guests!.Add(guestEntity);

                _dbContext.GuestBookings!.Add(new GuestBooking
                {
                    Booking = booking,
                    Guest = guestEntity,
                });
            }
        }

        await _dbContext.SaveChangesAsync( );

        var bookingResult = new CreateBookingDto
        {
            TravelerId = bookingRequest.TravelerId,
            GuestBookings = booking.GuestBookings,
            RoomId = booking.RoomId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
        };

        return ServiceResultDto<CreateBookingDto>.SuccessResult(bookingResult,"Reserva criada.");
    }

}
