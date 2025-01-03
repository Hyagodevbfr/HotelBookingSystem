using AutoMapper;
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
    private readonly IMapper _mapper;

    public TravelerService(AppDbContext dbContext, ITravelerVerifier travelerVerifier, UserManager<AppUser> userManager, IMapper mapper)
    {
        _dbContext = dbContext;
        _travelerVerifier = travelerVerifier;
        _userManager = userManager;
        _mapper = mapper;
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

    public async Task<ServiceResultDto<TravelerDetailDto>> GetTravelerDetail(string travelerId)
    {
        var travelerResult = (
                                from userDb in _userManager.Users
                                join travelerDb in _dbContext.Travelers! on userDb.Id equals travelerDb.UserId
                                where travelerDb.UserId == travelerId
                                select new TravelerDetailDto
                                {
                                    UserId = userDb.Id,
                                    FirstName = userDb.FirstName,
                                    LastName = userDb.LastName,
                                    Email = userDb.Email,
                                    PhoneNumber = userDb.PhoneNumber,
                                    BirthDate = userDb.BirthDate.ToString( ),
                                    IsActive = userDb.IsActive,
                                    NationalId = userDb.NationalId,
                                    RegistrationId = userDb.RegistrationId,
                                    Address = travelerDb.Address,
                                    City = travelerDb.City,
                                    State = travelerDb.State,
                                    PostalCode = travelerDb.PostalCode,
                                    Country = travelerDb.Country,
                                    EmergencyContact = travelerDb.EmergencyContact,
                                    EmergencyContactName = travelerDb.EmergencyContactName,
                                    HasEspecialNeeds = travelerDb.HasSpecialNeeds,
                                    SpecialNeedsDetails = travelerDb.SpecialNeedsDetails,
                                    DietaryPreferences = travelerDb.DietaryPreferences,
                                    CreatedOn = userDb.CreatedOn.ToString( ),
                                    EditedBy = userDb.EditedBy,
                                    EditedOn = userDb.EditedOn.ToString( ),
                                }
                             ).FirstOrDefaultAsync();

        if(travelerResult != null)
            return ServiceResultDto<TravelerDetailDto>.SuccessResult(await travelerResult,"Viajante localizado.");
        
        return ServiceResultDto<TravelerDetailDto>.Fail("Viajante não localizado.");
    }

    public async Task<ServiceResultDto<TravelerDetailDto>> GetSpecificTravelerDetail(string travelerId, string authenticatedUser)
    {
        var userWithPermission = await CheckUserAuthenticated(authenticatedUser,travelerId);
        if(!userWithPermission)
            return ServiceResultDto<TravelerDetailDto>.Fail("Esse usuário não tem permissão para editar o viajante");

        var travelerResult = (
                                from userDb in _userManager.Users
                                join travelerDb in _dbContext.Travelers! on userDb.Id equals travelerDb.UserId
                                where travelerDb.UserId == travelerId
                                select new TravelerDetailDto
                                {
                                    UserId = userDb.Id,
                                    FirstName = userDb.FirstName,
                                    LastName = userDb.LastName,
                                    Email = userDb.Email,
                                    PhoneNumber = userDb.PhoneNumber,
                                    BirthDate = userDb.BirthDate.ToString( ),
                                    IsActive = userDb.IsActive,
                                    NationalId = userDb.NationalId,
                                    RegistrationId = userDb.RegistrationId,
                                    Address = travelerDb.Address,
                                    City = travelerDb.City,
                                    State = travelerDb.State,
                                    PostalCode = travelerDb.PostalCode,
                                    Country = travelerDb.Country,
                                    EmergencyContact = travelerDb.EmergencyContact,
                                    EmergencyContactName = travelerDb.EmergencyContactName,
                                    HasEspecialNeeds = travelerDb.HasSpecialNeeds,
                                    SpecialNeedsDetails = travelerDb.SpecialNeedsDetails,
                                    DietaryPreferences = travelerDb.DietaryPreferences,
                                    CreatedOn = userDb.CreatedOn.ToString( ),
                                    EditedBy = userDb.EditedBy,
                                    EditedOn = userDb.EditedOn.ToString( ),
                                }
                             ).FirstOrDefaultAsync( );

        if(travelerResult != null)
            return ServiceResultDto<TravelerDetailDto>.SuccessResult(await travelerResult,"Viajante localizado.");

        return ServiceResultDto<TravelerDetailDto>.Fail("Viajante não localizado.");
    }

    public async Task<ServiceResultDto<List<TravelerDetailDto>>> GetTravelers()
    {
        var travelerList = await _dbContext.Travelers!.ToListAsync( );
        var travelerResult = await (
                                    from userDb in _userManager.Users
                                    join travelerDb in _dbContext.Travelers! on userDb.Id equals travelerDb.UserId
                                    select new TravelerDetailDto
                                    {
                                        UserId = userDb.Id,
                                        FirstName = userDb.FirstName,
                                        LastName = userDb.LastName,
                                        Email = userDb.Email,
                                        PhoneNumber = userDb.PhoneNumber,
                                        BirthDate = userDb.BirthDate.ToString( ),
                                        IsActive = userDb.IsActive,
                                        NationalId = userDb.NationalId,
                                        RegistrationId = userDb.RegistrationId,
                                        Address = travelerDb.Address,
                                        City = travelerDb.City,
                                        State = travelerDb.State,
                                        PostalCode = travelerDb.PostalCode,
                                        Country = travelerDb.Country,
                                        EmergencyContact = travelerDb.EmergencyContact,
                                        EmergencyContactName = travelerDb.EmergencyContactName,
                                        HasEspecialNeeds = travelerDb.HasSpecialNeeds,
                                        SpecialNeedsDetails = travelerDb.SpecialNeedsDetails,
                                        DietaryPreferences = travelerDb.DietaryPreferences,
                                        CreatedOn = userDb.CreatedOn.ToString( ),
                                        EditedBy = userDb.EditedBy,
                                        EditedOn = userDb.EditedOn.ToString( ),
                                    }
                                   ).ToListAsync( ); 

        var successResult = ServiceResultDto<List<TravelerDetailDto>>.SuccessResult(travelerResult,"Viajantes Localizados.");
        return successResult;
    }

    public async Task<ServiceResultDto<UpdateTravelerDto>> UpdateTraveler(UpdateTravelerDto updateTravelerDto, string userId, string authenticatedUser)
    {
        var userWithPermission = await CheckUserAuthenticated(authenticatedUser, userId);
        if(!userWithPermission)
            return ServiceResultDto<UpdateTravelerDto>.Fail("Esse usuário não tem permissão para editar o viajante");

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

        traveler.EditedBy = authenticatedUser;
        traveler.EditedOn = DateTime.Now;

        await _dbContext.SaveChangesAsync();
        var successResult = ServiceResultDto<UpdateTravelerDto>.SuccessResult(updateTravelerDto,"Viajante editado com sucesso.");

        return successResult;
     }

    public async Task<bool> CheckUserAuthenticated(string authenticatedUser, string userId)
    {
        if(authenticatedUser != userId)
        {
            var locateAuthenticatedUser = await _userManager.FindByIdAsync(authenticatedUser);
            var roles = await _userManager.GetRolesAsync(locateAuthenticatedUser!);
            if(!roles.Contains("Admin"))
                return false;
        }
        return true;
    }

}
