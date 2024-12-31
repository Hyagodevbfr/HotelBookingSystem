using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TravelerController: ControllerBase
{
    private readonly ITraveler _travelerService;
    private readonly IUserVerifier _userVerifier;
    private readonly AppDbContext _appDb;

    public TravelerController(ITraveler travelerService, IUserVerifier userVerifier, AppDbContext appDb)
    {
        _travelerService = travelerService;
        _userVerifier = userVerifier;
        _appDb = appDb;
    }
    [Authorize(Roles = "Admin, Employee")]
    [HttpPost]
    public async Task<IActionResult> CreateTraveler([FromBody]CreateTravelerDto createTravelerDto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(currentUserId is null)
            return NotFound(ServiceResultDto<UserDetailDto>.Fail("Usuário não encontrado."));


        var result = await _travelerService.CreateTraveler(createTravelerDto, currentUserId);
        if(!result.Success)
            return BadRequest(new { result.Message,result.Errors });

        return Ok(new { result.Message,Sucess = result.Success });
    }
    [Authorize(Roles = "Admin, Employee")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultDto<List<TravelerDetailDto>>>> GetTravelers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<List<TravelerDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _travelerService.GetTravelers();
        return Ok(result);
    }
    [HttpGet("detail")]
    public async Task<ActionResult<ServiceResultDto<TravelerDetailDto>>> GetUserDetail()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        var travelerFinded = await _appDb.Travelers!.FindAsync(currentUserId);
        if(travelerFinded is null)
            return NotFound(ServiceResultDto<TravelerDetailDto>.Fail("Viajante não encontrado."));

        var getDetail = await _travelerService.GetTravelerDetail(currentUserId!);
        return Ok(getDetail);
    }
    [HttpPatch("{id}")]
    public async Task<ActionResult<ServiceResultDto<UpdateTravelerDto>>> UpdateTraveler([FromBody] UpdateTravelerDto updateTravelerDto, string id) 
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(string.IsNullOrEmpty(currentUserId))
            return Unauthorized(ServiceResultDto<UpdateTravelerDto>.Fail("Usuário não autenticado."));

        var result = await _travelerService.UpdateTraveler(updateTravelerDto, id, currentUserId);
        if(!result.Success)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(ServiceResultDto<UpdateTravelerDto>.SuccessResult(updateTravelerDto, "Viajante cadastrado com sucesso."));

    }
}
