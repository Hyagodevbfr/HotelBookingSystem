using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin, Employee")]
public class RoomController: ControllerBase
{
    private readonly IRoom _roomService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserVerifier _userRoleVerifier;

    public RoomController(IRoom roomService,UserManager<AppUser> userManager,IUserVerifier userRoleVerifier)
    {
        _roomService = roomService;
        _userManager = userManager;
        _userRoleVerifier = userRoleVerifier;
    }
    [HttpPost]
    public async Task<ActionResult<ServiceResultDto<RoomDto>>> CreateRoom([FromBody] RoomDto roomDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roomService.CreateRoom(roomDto,currentUserId!);

        if(!result.Success)
            return BadRequest(result);

        return Ok(ServiceResultDto<RoomDto>.SuccessResult(roomDto,"Quarto criado com sucesso."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResultDto<RoomDetailDto>>> GetDetail(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roomService.GetRoom(id);
        if(!result.Success)
            return BadRequest(result);
        
        return Ok(ServiceResultDto<RoomDetailDto>.SuccessResult(result.Data,"Quarto encontrado."));
    }
    [HttpGet("rooms")]
    public async Task<ActionResult<ServiceResultDto<IEnumerable<RoomDetailDto>>>> GetRooms()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roomService.GetAllRooms();
        if(!result.Success)
            return BadRequest(result);

        return Ok((result));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ServiceResultDto<RoomDto>>> EditRoom([FromBody] RoomDto roomDto, int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roomService.EditRoom(roomDto,currentUserId!,id);
        if(!result.Success)
            return BadRequest(result);

        return Ok(ServiceResultDto<RoomDto>.SuccessResult(roomDto,"Quarto editado com sucesso."));
    }

    [AllowAnonymous]
    [HttpGet("AvailableRooms")]
    public async Task<ActionResult<RoomSearchResponse>> GetAvailableRooms([FromQuery] RoomSearchRequest searchRequest)
    {
        var result = await _roomService.GetAvaliableRooms(searchRequest);

        if(!result.Success)
        {
            return BadRequest(ServiceResultDto<RoomSearchResponse>.Fail("Erro ao buscar quartos vagos", result.Errors));
        }

        return Ok(result.Data);
    }
}
