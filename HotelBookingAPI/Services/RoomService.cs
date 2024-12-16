using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
