using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TravelerController: ControllerBase
{
    private readonly ITraveler _travelerService;

    public TravelerController(ITraveler travelerService)
    {
        _travelerService = travelerService;
    }
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
}
