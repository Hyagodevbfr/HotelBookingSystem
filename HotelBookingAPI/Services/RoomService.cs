using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class RoomService : IRoom
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public RoomService(UserManager<AppUser> userManager, AppDbContext dbContext, IMapper mapper)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ServiceResultDto<RoomDto>> CreateRoom(RoomDto roomDto,string userId)
    {
        var room = new Room(roomDto);
        room.CreatedBy = userId;

        if(!room.IsValid)
        {
            var result = ServiceResultDto<RoomDto>.Fail(
                "Não foi possível criar o quarto",
                room.Notifications.Select(n => n.Message)
            );

            return result;
        }
        TimeOnly checkinTime = room.CheckInTime  ?? new TimeOnly(14,0,0);
        TimeOnly checkoutTime = room.CheckOutTime  ?? new TimeOnly(11,0,0);

        await _dbContext.Rooms!.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var successResult = ServiceResultDto<RoomDto>.SuccessResult(roomDto,"Quarto criado com sucesso.");

        return await Task.FromResult(successResult);
    }

    public async Task<ServiceResultDto<RoomDto>> EditRoom(RoomDto room,string userId, int roomId)
    {
        var roomFinded = await _dbContext.Rooms!.Where(r => r.Id == roomId).FirstOrDefaultAsync( );
        if(roomFinded is null)
            return ServiceResultDto<RoomDto>.NullContent("Quarto não encontrado.");

        roomFinded.EditInfo(room);

        if(!roomFinded.IsValid)
        {
            var result = ServiceResultDto<RoomDto>.Fail(
                "Não foi possível criar o quarto",
                roomFinded.Notifications.Select(n => n.Message)
            );

            return result;
        }

        roomFinded.EditedBy = userId;
        roomFinded.EditedOn = DateTime.Now;

        await _dbContext.SaveChangesAsync( );
        var successResult = ServiceResultDto<RoomDto>.SuccessResult(room,"Quarto editado com sucesso.");

        return successResult;
    }

    public async Task<ServiceResultDto<IEnumerable<RoomDetailDto>>> GetAllRooms()
    {
        var rooms = await _dbContext.Rooms!.ToListAsync( );
        var roomDtos = rooms.Select(room => _mapper.Map<RoomDetailDto>(room)).ToList( );

        var successResult = ServiceResultDto<IEnumerable<RoomDetailDto>>.SuccessResult(roomDtos,"Quartos localizados.");
        return successResult;
    }

    public async Task<ServiceResultDto<IEnumerable<RoomSearchResponse>>> GetAvaliableRooms([FromBody] RoomSearchRequest searchRequest)
    {
        var validateRoomSearch = ValidateRoomSearch(searchRequest);
        if(validateRoomSearch != null)
            return ServiceResultDto<IEnumerable<RoomSearchResponse>>.Fail(validateRoomSearch);

    var availableRooms = await FetchAvailableRoomAsync(searchRequest);

    if (!availableRooms.Any())
        return ServiceResultDto<IEnumerable<RoomSearchResponse>>.Fail("Nenhum quarto disponível para os critérios especificados.");
    
    return ServiceResultDto<IEnumerable<RoomSearchResponse>>.SuccessResult(availableRooms, "Quartos Localizados");
}

    public async Task<ServiceResultDto<DetailsAvailableRoomDto>> GetDetailsAvailableRoom(int id, string userId,[FromBody] RoomSearchRequest searchRequest)
    {
        var traveler = await _dbContext.Travelers!.FindAsync(userId);
        if(traveler is null)
            return ServiceResultDto<DetailsAvailableRoomDto>.NullContent("Usuário não possui perfil de viajante.");

        var avaliableRoom = await FetchAvailableRoomAsync(searchRequest);
        var room = avaliableRoom.FirstOrDefault(r => r.roomId == id);
        if(avaliableRoom is null || !avaliableRoom.Any(r => r.roomId == id))
            return ServiceResultDto<DetailsAvailableRoomDto>.Fail("Quarto não localizado.");

        var nights = (searchRequest.CheckOutDate - searchRequest.CheckInDate).Days;
        var totalStay = nights * room!.PricePerNight;

        var response = new DetailsAvailableRoomDto
        {
            Id = room.roomId,
            RoomName = room.RoomName,
            Capacity = room.Capacity,
            PricePerNight = room.PricePerNight,
            TotalStay = totalStay,
            Nights = nights,
            Guests = searchRequest.AdultCapacity + searchRequest.ChildCapacity
        };
        
        return ServiceResultDto<DetailsAvailableRoomDto>.SuccessResult(response, "Quarto Localizado.");
    }

    public async Task<ServiceResultDto<RoomDetailDto>> GetRoom(int roomId)
    {
        var room = await _dbContext.Rooms!.FindAsync(roomId);
        var roomDto = _mapper.Map<RoomDetailDto>(room);
        if(roomDto is null)
            return ServiceResultDto<RoomDetailDto>.NullContent("Quarto não encontrado.");

        var result = ServiceResultDto<RoomDetailDto>.SuccessResult(roomDto,"Quarto encontrado.");

        return result;
    }

    private string? ValidateRoomSearch(RoomSearchRequest searchRequest)
    {
        if(searchRequest.CheckInDate >= searchRequest.CheckOutDate || searchRequest.CheckInDate == default ||
            searchRequest.CheckInDate < DateTime.Now || searchRequest.CheckOutDate < DateTime.Now)
            return "Datas de check-in e check-out inválidas.";

        if(searchRequest.AdultCapacity <= 0 || searchRequest.ChildCapacity < 0)
            return "Capacidade inválida.";

        return null;
    }

    private async Task<List<RoomSearchResponse>> FetchAvailableRoomAsync(RoomSearchRequest searchRequest)
    {
        return await _dbContext.Rooms!
                        .Where(room => !_dbContext.Bookings
                            .Any(b => b.RoomId == room.Id &&
                                        b.CheckInDate < searchRequest.CheckOutDate &&
                                        b.CheckOutDate > searchRequest.CheckInDate) &&
                                        room.Capacity >= (searchRequest.AdultCapacity + searchRequest.ChildCapacity) &&
                                        room.IsAvailable)
                        .Select(room => new RoomSearchResponse(
                                room.Id,
                                room.RoomName!,
                                room.Capacity,
                                room.PricePerNight,
                                EF.Functions.DateDiffDay(searchRequest.CheckInDate,searchRequest.CheckOutDate),
                                room.PricePerNight * EF.Functions.DateDiffDay(searchRequest.CheckInDate,searchRequest.CheckOutDate)
                            ))
                        .ToListAsync( );
    }
}
