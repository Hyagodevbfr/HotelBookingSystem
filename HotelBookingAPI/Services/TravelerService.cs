using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services;

public class TravelerService: ITraveler
{
    private readonly AppDbContext _dbContext;
    private readonly ITravelerVerifier _travelerVerifier;

    public TravelerService(AppDbContext dbContext, ITravelerVerifier travelerVerifier)
    {
        _dbContext = dbContext;
        _travelerVerifier = travelerVerifier;
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
}
