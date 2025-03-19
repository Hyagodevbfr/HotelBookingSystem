using HotelBookingAPI.Dtos;
using HotelBookingAPI.Enums;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using RabbitPublisher;
using Shared;
using static Shared.BookingSharedDto;


namespace HotelBookingAPI.Services;

public class BookingService: IBooking
{
    private readonly AppDbContext _dbContext;
    private readonly PublisherRabbitMq _rabbitPublisher;

    public BookingService(AppDbContext dbContext, PublisherRabbitMq rabbitPublisher)
    {
        _dbContext = dbContext;
        _rabbitPublisher = rabbitPublisher;
    }
    public async Task<ServiceResultDto<CreateBookingDto>> CreateBooking(BookingRequest bookingRequest)
    {
        var traveler = await _dbContext.Travelers!.FirstOrDefaultAsync(t => t.UserId == bookingRequest.TravelerId);
        if(traveler is null)
            return ServiceResultDto<CreateBookingDto>.Fail("Viajante não localizado.");

        var user = await _dbContext.Users.FindAsync(bookingRequest.TravelerId);

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

        var bookingToShared = new BookingShared
        {
            BookingId = booking.Id,
            TravelerFullName = $"{user!.FirstName} {user.LastName}",
            TravelerNationalId = user.NationalId,
            TravelerEmail = user.Email!,
            TypeRoom = booking.Room?.Type?.ToString( ) ?? "Falha ao recuperar tipo de quarto",
            RoomName = booking.Room?.RoomName ?? "Falha ao recuperar nome do quarto",
            CheckIn = booking.CheckInDate.ToString( ) ?? "Falha ao recuperar data de check-in",
            CheckOut = booking.CheckOutDate.ToString( ) ?? "Falha ao recuperar data de check-out",
            Status = booking.Status.ToString( ) ?? "Falha ao recuperar status da reserva",
            TotalPrice = $"R${(Decimal)booking.TotalPrice}"
        };

        await _rabbitPublisher.SendBookingAsync(bookingToShared);

        var bookingResult = new CreateBookingDto
        {
            TravelerId = bookingRequest.TravelerId,
            GuestBookings = booking.GuestBookings!.Select(gb => new GuestBookingDto
            {
                BookingId = gb.BookingId,
                GuestId = gb.GuestId,
                FirstName = gb.Guest.FirstName,
                LastName = gb.Guest.LastName,
                RegistrationId = gb.Guest.RegistrationId,
                BirthDate = gb.Guest.BirthDate.ToString("dd-MM-yyyy"),
                HasSpecialNeeds = gb.Guest.HasSpecialNeeds,
                SpecialNeedsDetails = gb.Guest.SpecialNeedsDetails,
                DietaryPreferences = gb.Guest.DietaryPreferences,
            }).ToList( ),
            RoomId = booking.RoomId,
            CheckInDate = bookingRequest.CheckInDate.ToString("dd-MM-yyyy"),
            CheckOutDate = bookingRequest.CheckOutDate.ToString("dd-MM-yyyy"),
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
        var bookingsDto = bookings.Select(booking => new BookingDto
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


        var successResult = ServiceResultDto<List<BookingDto>>.SuccessResult(bookingsDto,"Quartos localizados.");
        return successResult;
    }

    public async Task<ServiceResultDto<BookingDto>> GetBooking(int id)
    {
        var booking = await _dbContext.Bookings!.Where(b => b.Id == id).Include(b => b.Traveler).ThenInclude(t => t!.User).Include(b => b.GuestBookings)!.ThenInclude(gb => gb.Guest).FirstOrDefaultAsync( );
        if(booking is null)
            return ServiceResultDto<BookingDto>.NullContent("Reserva não localizada.");

        var bookingDto = new BookingDto
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
        };

        var successReult = ServiceResultDto<BookingDto>.SuccessResult(bookingDto,"Reserva localizada.");

        return successReult;
    }

    public async Task<ServiceResultDto<BookingDto>> GetBookingByTravelerNationalId(string nationalId)
    {
        var booking = await _dbContext.Bookings!
        .Include(b => b.Traveler)
        .ThenInclude(t => t!.User)
        .Include(b => b.GuestBookings)!
        .ThenInclude(gb => gb.Guest)
        .Where(b => b.Traveler != null && b.Traveler.User != null && b.Traveler.User.NationalId == nationalId && b.Status == BookingStatus.Confirmed)
        .FirstOrDefaultAsync( );
        if(booking == null)
            return ServiceResultDto<BookingDto>.NullContent("Não existe reserva confirmada para esse hóspede.");

        var bookingDto = new BookingDto
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
        };

        var successReult = ServiceResultDto<BookingDto>.SuccessResult(bookingDto,"Reserva localizada.");

        return successReult;
    }

    public async Task<ServiceResultDto<List<BookingDto>>> GetBookingsByStatus(BookingStatus bookingStatus)
    {
        var bookings = await _dbContext.Bookings!.Where(b => b.Status == bookingStatus).Include(b => b.Traveler).ThenInclude(t => t!.User).Include(b => b.GuestBookings)!.ThenInclude(gb => gb.Guest).ToListAsync( );
        var bookingsDto = bookings.Select(booking => new BookingDto
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
        if(!bookingsDto.Any( ))
            return ServiceResultDto<List<BookingDto>>.NullContent($"Não há quartos com o status {bookingStatus}");

        var successResult = ServiceResultDto<List<BookingDto>>.SuccessResult(bookingsDto,"Quartos localizados.");
        return successResult;
    }

    public async Task<ServiceResultDto<string>> UpdateBookingStatus(int id,string userId,BookingStatus bookingStatus)
    {
        var booking = await _dbContext.Bookings!.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
        if(booking is null)
            return ServiceResultDto<string>.NullContent("Reserva não localizada.");

        var user = await _dbContext.Users.FindAsync(booking.TravelerId);

        booking.Status = bookingStatus;
        if(booking.Status == BookingStatus.Cancelled)
        {
            var room = await _dbContext.Rooms!.FirstOrDefaultAsync(r => r.Id == booking.RoomId);
            if(room is not null)
            {
                room.RoomsQuantity += 1;
                booking.TotalPrice = Math.Abs(booking.TotalPrice) * -1;
                _dbContext.SaveChanges( );
            }
            else
                return ServiceResultDto<string>.NullContent("Quarto não localizado.");
        }
        if(booking.Status == BookingStatus.CheckoutCompleted || booking.Status == BookingStatus.CheckinCompleted)
        {
            return ServiceResultDto<string>.Fail("Não é possível alterar para esse status nessa página.");
        }
        booking.EditedAt = DateTime.Now;
        booking.EditedBy = userId;
        await _dbContext.SaveChangesAsync( );

        var bookingToShared = new BookingShared
        {
            BookingId = booking.Id,
            TravelerFullName = $"{user!.FirstName} {user.LastName}",
            TravelerNationalId = user.NationalId,
            TravelerEmail = user.Email!,
            TypeRoom = booking.Room?.Type?.ToString( ) ?? "Falha ao recuperar tipo de quarto",
            RoomName = booking.Room?.RoomName ?? "Falha ao recuperar nome do quarto",
            CheckIn = booking.CheckInDate.ToString( ) ?? "Falha ao recuperar data de check-in",
            CheckOut = booking.CheckOutDate.ToString( ) ?? "Falha ao recuperar data de check-out",
            Status = booking.Status.ToString( ) ?? "Falha ao recuperar status da reserva",
            TotalPrice = $"R${(Decimal)booking.TotalPrice}"
        };

        await _rabbitPublisher.StatusUpdate(bookingToShared);

        var successResult = ServiceResultDto<string>.SuccessResult("O status da reserva foi atualizado com sucesso.","Status atualizado.");

        return successResult;
    }

    public async Task<ServiceResultDto<string>> Checkin(int bookingId, bool confirmAction = false)
    {
        var booking = await _dbContext.Bookings!.FirstOrDefaultAsync(b => b.Id == bookingId);
        if(booking is null)
            return ServiceResultDto<string>.NullContent("Reserva não localizada.");

        if(booking.Status != BookingStatus.Confirmed)
            return ServiceResultDto<string>.Fail("O status da reserva não permite realizar o check-in.");

        DateOnly concatenatedCheckInDate = DateOnly.FromDateTime(DateTime.Now);
        TimeOnly concatenatedCheckInTime = TimeOnly.FromDateTime(DateTime.Now);

        DateTime ajustedCheckinDate = new DateTime(
            concatenatedCheckInDate.Year,
            concatenatedCheckInDate.Month,
            concatenatedCheckInDate.Day,
            concatenatedCheckInTime.Hour,
            concatenatedCheckInTime.Minute,
            concatenatedCheckInTime.Second,
            concatenatedCheckInTime.Millisecond
        );
        TimeSpan diff = booking.CheckInDate - ajustedCheckinDate;
        if( diff.TotalHours < 3 )
        {
            double totalHours = diff.TotalHours;
            int days = (int)(totalHours / 24);
            int hours = (int)(totalHours % 24);
            int minutes = diff.Minutes;

            string timeRemaining = days > 0
                ? $"{days} dias, {hours} horas e {minutes} minutos"
                : hours > 0
                    ? $"{hours} horas e {minutes} minutos"
                    : $"{minutes} minutos";
                        
            booking.Status = BookingStatus.CheckinCompleted;
            _dbContext.SaveChanges();
        }
        else if(confirmAction == false && diff.TotalHours > 0)
        {
                return ServiceResultDto<string>.Fail($"Tentativa de checkin antes de horario marcado sem confirmação. {diff}");
        }       

        booking.Status = BookingStatus.CheckinCompleted;
        _dbContext.SaveChanges( );

        return ServiceResultDto<string>.SuccessResult("Checkin realizado com sucesso.","Checkin realizado.");
    }

    public async Task<ServiceResultDto<string>> Checkout(int bookingId,bool confirmAction)
    {
        var booking = await _dbContext.Bookings!.FirstOrDefaultAsync(b => b.Id == bookingId);
        if(booking is null)
            return ServiceResultDto<string>.NullContent("Reserva não localizada.");

        var room = await _dbContext.Rooms!.FirstOrDefaultAsync(r => r.Id == booking.RoomId);
        if(room is null)
            return ServiceResultDto<string>.NullContent("Quarto não localizado.");

        if(booking.Status != BookingStatus.CheckinCompleted)
            return ServiceResultDto<string>.Fail("O status da reserva não permite realizar o check-out.");

        DateOnly concatenatedCheckInDate = DateOnly.FromDateTime(DateTime.Now);
        TimeOnly concatenatedCheckInTime = TimeOnly.FromDateTime(DateTime.Now);

        DateTime ajustedCheckinDate = new DateTime(
            concatenatedCheckInDate.Year,
            concatenatedCheckInDate.Month,
            concatenatedCheckInDate.Day,
            concatenatedCheckInTime.Hour,
            concatenatedCheckInTime.Minute,
            concatenatedCheckInTime.Second,
            concatenatedCheckInTime.Millisecond
        );
        TimeSpan diff = booking.CheckInDate - ajustedCheckinDate;
        if(diff.TotalHours > 3)
        {
            if(confirmAction == false)
            {
                double totalHours = diff.TotalHours;
                int days = (int)(totalHours / 24);
                int hours = (int)(totalHours % 24);
                int minutes = diff.Minutes;

                string timeRemaining = days > 0
                    ? $"{days} dias, {hours} horas e {minutes} minutos"
                    : hours > 0
                        ? $"{hours} horas e {minutes} minutos"
                        : $"{minutes} minutos";
                return ServiceResultDto<string>.Fail($"Check-out ultrapassou {timeRemaining}.");
            }

            booking.Status = BookingStatus.CheckoutCompleted;
            room.RoomsQuantity += 1;            
            _dbContext.SaveChanges( );
        }else if (booking.CheckOutDate >  ajustedCheckinDate || booking.CheckInDate < ajustedCheckinDate)
            return ServiceResultDto<string>.Fail("O dia de hoje é anterior a data de check-out. Não foi possível realizar o check-out.");

        booking.Status = BookingStatus.CheckoutCompleted;
        room.RoomsQuantity += 1;
        _dbContext.SaveChanges( );
        return ServiceResultDto<string>.SuccessResult("Check-out realizado com sucesso.","Check-out realizado.");
    }
}


