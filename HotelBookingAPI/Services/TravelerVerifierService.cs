using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class TravelerVerifierService: ITravelerVerifier
{
    private readonly AppDbContext _appDb;

    public TravelerVerifierService(AppDbContext appDb)
    {
        _appDb = appDb;
    }
    public async Task<ServiceResultDto<Traveler>> IsDuplicateTraveler(object travelerDto, string travelerId)
    {
        string message = string.Empty;
        Traveler? traveler = travelerDto switch
        {
            CreateTravelerDto createTraveler => await _appDb.Travelers!.FirstOrDefaultAsync(t => t.UserId == travelerId),

            _ => throw new ArgumentException("Não foi possível concluir a validação.")
        };

        if(traveler is null)
            return ServiceResultDto<Traveler>.SuccessResult(null,"Não há duplicidade.");

        switch(travelerDto)
        {
            case CreateTravelerDto createTraveler:
                if(traveler.UserId == travelerId)
                    message += "Úsuário já cadastrado como viajante.";
            break;
        }

        return ServiceResultDto<Traveler>.Fail(message.Trim( ));
    }
}
