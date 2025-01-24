﻿using AutoMapper;
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

        DateTime concatenatedCheckInDate = bookingRequest.CheckInDate;
        TimeOnly concatenatedCheckInTime = (TimeOnly)room.CheckInTime!;
        DateTime concatenadCheckOutDate = bookingRequest.CheckOutDate;
        TimeOnly concatenadCheckOutTime = (TimeOnly)room.CheckOutTime!;

        DateTime ajustedCheckinDate = new DateTime(
            concatenatedCheckInDate.Year,
            concatenatedCheckInDate.Month,
            concatenatedCheckInDate.Day,
            concatenatedCheckInTime.Hour,
            concatenatedCheckInTime.Minute,
            concatenatedCheckInTime.Second,
            concatenatedCheckInTime.Millisecond
        );

        DateTime ajustedCheckoutDate = new DateTime(
            concatenadCheckOutDate.Year,
            concatenadCheckOutDate.Month,
            concatenadCheckOutDate.Day,
            concatenadCheckOutTime.Hour,
            concatenadCheckOutTime.Minute,
            concatenadCheckOutTime.Second,
            concatenadCheckOutTime.Millisecond
        );

        TimeSpan totalDays = bookingRequest.CheckOutDate - bookingRequest.CheckInDate;
        var totalPriceBooking = (decimal)room.PricePerNight * (decimal)totalDays.Days;

        var booking = new Booking
        {
            TravelerId = bookingRequest.TravelerId,
            RoomId = bookingRequest.RoomId,
            CheckInDate = ajustedCheckinDate,
            CheckOutDate = ajustedCheckoutDate,
            TotalPrice = totalPriceBooking,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            CreatedBy = bookingRequest.TravelerId,
            EditedBy = bookingRequest.TravelerId,
        };

        _dbContext.Bookings!.Add(booking);
        if(room.RoomsQuantity < 0)
            return ServiceResultDto<CreateBookingDto>.Fail($"Quarto não disponível.");
        room.RoomsQuantity -= 1;

        var guestBookings = new List<GuestBookingDto>( );
        if(bookingRequest.Guests != null)
        {
            foreach(var guest in bookingRequest.Guests)
            {
                var guestExist = await _dbContext.Guests!.FirstOrDefaultAsync(g => g.RegistrationId == guest.RegistrationId && g.TravelerId == traveler.UserId);
                Guest guestEntity;

                if(guestExist != null)
                {
                    guestEntity = guestExist;
                }
                else
                {
                    guestEntity = new Guest
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

                    await _dbContext.Guests!.AddAsync(guestEntity);
                }

                var guestBooking = new GuestBooking
                {
                    Booking = booking,
                    Guest = guestEntity,
                };
                await _dbContext.GuestBookings!.AddAsync(guestBooking);

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

        var bookingResult = new CreateBookingDto
        {
            TravelerId = bookingRequest.TravelerId,
            GuestBookings = booking.GuestBookings,
            RoomId = booking.RoomId,
            CheckInDate = bookingRequest.CheckInDate,
            CheckOutDate = bookingRequest.CheckOutDate,
            CheckInTime = room.CheckInTime,
            CheckOutTime = room.CheckOutTime,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
        };

        return ServiceResultDto<CreateBookingDto>.SuccessResult(bookingResult,"Reserva criada com sucesso.");
    }

    public async Task<ServiceResultDto<List<BookingDto>>> GetAllBookings()
    {
        var bookings = await _dbContext.Bookings!.Include(b => b.Traveler).ThenInclude(t => t!.User).Include(b => b.GuestBookings)!.ThenInclude(gb => gb.Guest).ToListAsync( );
        var bookingDtos = bookings.Select(booking => new BookingDto
        {
            Id = booking.Id,
            TravelerId = booking.TravelerId,
            TravelerFullName = $"{booking.Traveler!.User.FirstName} {booking.Traveler.User.LastName}",
            TravelerNationalId = booking.Traveler.User.NationalId,
            Guests = booking.GuestBookings!.Select(gb => new GuestDto
            {
                FirstName = gb.Guest.FirstName,
                LastName = gb.Guest.LastName,
                RegistrationId = gb.Guest.RegistrationId,
                BirthDate = gb.Guest.BirthDate,
                HasSpecialNeeds = gb.Guest.HasSpecialNeeds,
                SpecialNeedsDetails = gb.Guest.SpecialNeedsDetails,
                DietaryPreferences = gb.Guest.DietaryPreferences,
            }).ToList( ),
            RoomId = booking.RoomId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status
        }).ToList( );


        var successResult = ServiceResultDto<List<BookingDto>>.SuccessResult(bookingDtos,"Quartos localizados.");
        return successResult;
    }
}
