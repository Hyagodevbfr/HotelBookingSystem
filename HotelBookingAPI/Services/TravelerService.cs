using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class TravelerService: ITraveler
{
    private readonly AppDbContext _dbContext;
    private readonly ITravelerVerifier _travelerVerifier;
    private readonly UserManager<AppUser> _userManager;

    public TravelerService(AppDbContext dbContext, ITravelerVerifier travelerVerifier, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _travelerVerifier = travelerVerifier;
        _userManager = userManager;
    }
    public async Task<ServiceResultDto<CreateTravelerDto>> CreateTraveler(CreateTravelerDto createTravelerDto, string userId)
    {
        var traveler = new Traveler(createTravelerDto);
        traveler.UserId = userId;
        traveler.CreatedBy = userId;
        traveler.CreatedOn = DateTime.Now;

        var isDuplicatedTraveler = await _travelerVerifier.IsDuplicateTraveler(createTravelerDto, userId);
        if(!isDuplicatedTraveler.Success)
            return ServiceResultDto<CreateTravelerDto>.Fail("Não foi possível cadastrar o viajante.", [isDuplicatedTraveler.Message]);

        if(!traveler.IsValid)
        {
            var result = ServiceResultDto<CreateTravelerDto>.Fail("Não foi possível cadastrar o viajante",traveler.Notifications.Select(n => n.Message));
            return result;
        }

        await _dbContext.Travelers!.AddAsync(traveler);
        await _dbContext.SaveChangesAsync();

        var successResult = ServiceResultDto<CreateTravelerDto>.SuccessResult(createTravelerDto,"Viajante criado com sucesso.");

        return await Task.FromResult(successResult);
    }

    public async Task<ServiceResultDto<UpdateTravelerDto>> UpdateTraveler(UpdateTravelerDto updateTravelerDto, string userId)
    {
        //var user = await _userManager.FindByIdAsync(userId);
        Traveler? traveler = await _dbContext.Travelers!.Where(u => u.UserId == userId).FirstOrDefaultAsync( );
        if(traveler is null)
        {
            var result = ServiceResultDto<UpdateTravelerDto>.NullContent("Viajante não localizado.");
            return result;
        }

        traveler.EditTraveler(updateTravelerDto);

        if(!traveler.IsValid)
        {
            var result = ServiceResultDto<UpdateTravelerDto>.Fail("Não foi possível editar as informações do viajante.", traveler.Notifications.Select(n => n.Message));
            return result;
        }
        traveler.EditedBy = userId;
        traveler.EditedOn = DateTime.Now;

        await _dbContext.SaveChangesAsync();
        var successResult = ServiceResultDto<UpdateTravelerDto>.SuccessResult(updateTravelerDto,"Viajante editado com sucesso.");

        return successResult;
    }
}
