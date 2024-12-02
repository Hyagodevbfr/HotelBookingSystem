using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin, Employee")]
public class RoleController: ControllerBase
{
    private readonly IUserVerifierService _userRoleVerifier;
    private readonly IRole _roleService;
    private readonly UserManager<AppUser> _userManager;

    public RoleController
        (
        IRole roleService,
        UserManager<AppUser> userManager,
        IUserVerifierService userRoleVerifier
        )
    {
        _userRoleVerifier = userRoleVerifier;
        _roleService = roleService;
        _userManager = userManager;
    }
    [HttpPost("create")]
    public async Task<ActionResult<ServiceResultDto<CreateRoleDto>>> CreateRole([FromBody] CreateRoleDto roleDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roleService.CreateRole(roleDto);
        if(!result.Success)
            return BadRequest(new { result.Message,result.Errors });

        return Ok(new { result.Message,Sucess = result.Success });
    }
    [HttpGet("roles")]
    public async Task<ActionResult<ServiceResultDto<IEnumerable<RoleResponseDto>>>> GetRoles()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roleService.GetRoles( );
        return Ok(result);
    }
    [HttpGet("delete/{roleId}")]
    public async Task<ActionResult<ServiceResultDto<IdentityRole>>> DeleteRole(string roleId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roleService.DeleteRole(roleId);
        return Ok(result);
    }
    [HttpPut("assign")]
    public async Task<ActionResult<ServiceResultDto<IdentityRole>>> AssignRole([FromBody] AssignRoleDto assignRole)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _roleService.AssignRole(assignRole);
        return Ok(result);
    }
}
