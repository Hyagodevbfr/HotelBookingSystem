using HotelBookingAPI.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IRole
{
    Task<ServiceResultDto<CreateRoleDto>> CreateRole(CreateRoleDto role);
    Task<ServiceResultDto<IEnumerable<RoleResponseDto>>> GetRoles();
    Task<ActionResult<ServiceResultDto<IdentityRole>>> DeleteRole(string roleId);
    Task<ActionResult<ServiceResultDto<IdentityRole>>> AssignRole(AssignRoleDto assignRole);
}
