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

        int actualGuestCount = (bookingRequest.Guests?.Count ?? 0) + 1;
        if(actualGuestCount != bookingRequest.TotalGuests)
        {
            return ServiceResultDto<CreateBookingDto>.Fail($"Número de hóspedes inválido. Esperado: {bookingRequest.TotalGuests}, Recebido: {actualGuestCount}.");
        }

        if(bookingRequest.ChildCapacity > 0)
        {
            var children = bookingRequest.Guests?.Where(g => g.BirthDate.AddYears(13) > DateTime.Now).ToList( );

            if(children == null || children.Count != bookingRequest.ChildCapacity)
            {
                return ServiceResultDto<CreateBookingDto>.Fail($"Número de crianças inválido. Esperado: {bookingRequest.ChildCapacity}, Recebido: {children?.Count ?? 0}.");
            }
        }

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

        var guestBookings = new List<GuestBookingDto>( );
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
                    CreatedOn = DateTime.Now
                };
                _dbContext.Guests!.Add(guestEntity);

                var guestBooking = new GuestBooking
                {
                    Booking = booking,
                    Guest = guestEntity,
                };
                _dbContext.GuestBookings!.Add(guestBooking);

                guestBookings.Add(new GuestBookingDto
                {
                    BookingId = booking.Id,
                    GuestId = guestEntity.Id
                });
            }
        }

        var bookingHistory = await _dbContext.BookingHistories!
        .FirstOrDefaultAsync(bh => bh.TravelerId == bookingRequest.TravelerId);

        if(bookingHistory == null)
        {
            bookingHistory = new BookingHistory
            {
                Id = Guid.NewGuid( ),
                TravelerId = bookingRequest.TravelerId,
                Bookings = new List<Booking> { booking }
            };
            _dbContext.BookingHistories!.Add(bookingHistory);
        }
        else
        {
            bookingHistory.Bookings ??= new List<Booking>( );
            bookingHistory.Bookings.Add(booking);
        }

        await _dbContext.SaveChangesAsync( );

        DateTime concatenatedCheckinDate = booking.CheckInDate;
        TimeOnly concatenatedCheckinTime = (TimeOnly)room.CheckInTime!;
        DateTime concatenadCheckoutDate = booking.CheckOutDate;
        TimeOnly concatenadCheckoutTime = (TimeOnly)room.CheckOutTime!;

        DateTime ajustedCheckinDate = new DateTime(
            concatenatedCheckinDate.Year,
            concatenatedCheckinDate.Month,
            concatenatedCheckinDate.Day,
            concatenatedCheckinTime.Hour,
            concatenatedCheckinTime.Minute,
            concatenatedCheckinTime.Second,
            concatenatedCheckinTime.Millisecond
            );
        
        DateTime ajustedCheckoutDate = new DateTime(
            concatenadCheckoutDate.Year,
            concatenadCheckoutDate.Month,
            concatenadCheckoutDate.Day,
            concatenadCheckoutDate.Hour,
            concatenadCheckoutDate.Minute,
            concatenadCheckoutDate.Second,
            concatenadCheckoutDate.Millisecond
            );


        var bookingResult = new CreateBookingDto
        {
            TravelerId = bookingRequest.TravelerId,
            GuestBookings = booking.GuestBookings,
            RoomId = booking.RoomId,
            CheckInDate = ajustedCheckinDate,
            CheckOutDate = ajustedCheckoutDate,
            CheckInTime = room.CheckInTime,
            CheckOutTime = room.CheckOutTime,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
        };

        return ServiceResultDto<CreateBookingDto>.SuccessResult(bookingResult,"Reserva criada.");
    }


}
