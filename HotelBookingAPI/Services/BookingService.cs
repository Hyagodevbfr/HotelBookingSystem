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
        if(room is null)
            return ServiceResultDto<CreateBookingDto>.Fail("Quarto não localizado.");

        var booking = new Booking
        {
            TravelerId = bookingRequest.TravelerId,
            Traveler = traveler,
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
        booking.Room = room;

        _dbContext.Bookings!.Add(booking);
        booking.Room.RoomsQuantity = room.RoomsQuantity - 1;
        await _dbContext.SaveChangesAsync( );

        if(bookingRequest.Guests != null)
        {
            foreach(var guest in bookingRequest.Guests)
            {
                var guestFinded = new Guest
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
                _dbContext.Guests!.Add(guestFinded);
                await _dbContext.SaveChangesAsync( );

                var bookingObject = await _dbContext.Bookings!.FirstAsync(bo => bo.Id == booking.Id);
                var guestObject = await _dbContext.Guests!.FirstAsync(go =>  go.Id == guestFinded.Id);
                var guestBooking = new GuestBooking
                {
                    BookingId = booking.Id,
                    Booking = bookingObject,
                    GuestId = guestFinded.Id,
                    Guest = guestObject
                };
                booking.GuestBookings!.Add(guestBooking);
            }
        }
        await _dbContext.SaveChangesAsync();

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
