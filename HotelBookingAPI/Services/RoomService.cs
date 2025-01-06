using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.WebUtilities;

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
    if (searchRequest.CheckInDate >= searchRequest.CheckOutDate || searchRequest.CheckInDate == default ||
            searchRequest.CheckInDate < DateTime.Now || searchRequest.CheckOutDate < DateTime.Now)
        return ServiceResultDto<IEnumerable<RoomSearchResponse>>.Fail("Datas de check-in e check-out inválidas.");
    
    if (searchRequest.AdultCapacity <= 0 || searchRequest.ChildCapacity < 0)
        return ServiceResultDto<IEnumerable<RoomSearchResponse>>.Fail("Capacidade inválida.");

    var availableRooms = await _dbContext.Rooms!
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
        .ToListAsync();

    if (!availableRooms.Any())
        return ServiceResultDto<IEnumerable<RoomSearchResponse>>.Fail("Nenhum quarto disponível para os critérios especificados.");
    
    return ServiceResultDto<IEnumerable<RoomSearchResponse>>.SuccessResult(availableRooms, "Quartos Localizados");
}

    public async Task<ServiceResultDto<DetailsAvailableRoom>> GetDetailsAvailableRoom(int id, [FromQuery] string queries)
    {
        if(string.IsNullOrEmpty(queries))
            return ServiceResultDto<DetailsAvailableRoom>.Fail("Não foi possível localizar os detalhes do quarto.");

        var queryParameters = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queries);

        if(!queryParameters.TryGetValue("CheckInDate",out var checkInDate) ||
        !queryParameters.TryGetValue("CheckOutDate",out var checkOutDate) ||
        !queryParameters.TryGetValue("AdultCapacity",out var adultCapacity) ||
        !queryParameters.TryGetValue("ChildCapacity",out var childCapacity))
            return ServiceResultDto<DetailsAvailableRoom>.Fail("As queries fornecidas são inválidas ou estão incompletas.");

        if(!DateTime.TryParse(checkInDate,out var parsedCheckInDate) ||
        !DateTime.TryParse(checkOutDate,out var parsedCheckOutDate) ||
        !int.TryParse(adultCapacity,out var parsedAdultCapacity) ||
        !int.TryParse(childCapacity,out var parsedChildCapacity))
            return ServiceResultDto<DetailsAvailableRoom>.Fail("Os parâmetros das queries são inválidos.");

        var avaliableRoom = await _dbContext.Rooms!.FirstOrDefaultAsync(r => r.Id == id);
        if(avaliableRoom is null)
            return ServiceResultDto<DetailsAvailableRoom>.Fail("Quarto não localizado.");

        var nights = (parsedCheckOutDate - parsedCheckInDate).Days;
        var totalStay = nights * avaliableRoom.PricePerNight;

        var response = _mapper.Map<DetailsAvailableRoom>(avaliableRoom);
        response.TotalStay = totalStay;
        response.Nights = nights;
        response.Guests = parsedAdultCapacity + parsedChildCapacity;

        return ServiceResultDto<DetailsAvailableRoom>.SuccessResult(response, "Quarto Localizado.");
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
}
