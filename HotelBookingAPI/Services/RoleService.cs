using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HotelBookingAPI.Services;

public class RoleService: IRole
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ActionResult<ServiceResultDto<IdentityRole>>> AssignRole(AssignRoleDto assignRole)
    {
        var user = await _userManager.FindByIdAsync(assignRole.UserId);
        if (user is null)
            return ServiceResultDto<IdentityRole>.Fail("Usuário não encontrado.");

        var role = await _roleManager.FindByIdAsync(assignRole.RoleId);
        if(role is null)
            return ServiceResultDto<IdentityRole>.Fail("Papél não encontrado.");

        var userRoles = await _userManager.GetRolesAsync(user);
        if(userRoles.Any() && !userRoles.Contains(role.Name!))
        {
            var removeRoles = await _userManager.RemoveFromRolesAsync(user,userRoles);
            if(!removeRoles.Succeeded)
                return ServiceResultDto<IdentityRole>.Fail("Falha ao remover papél existente.");
        }
        var result = await _userManager.AddToRoleAsync(user,role.Name!);
        if(result.Succeeded)
            return ServiceResultDto<IdentityRole>.SuccessResult(null,$"{role.Name} foi atribuído ao usuário:{user.FirstName} {user.LastName}");

        var error = result.Errors.FirstOrDefault();

        return ServiceResultDto<IdentityRole>.Fail(error!.Description);


    }

    public async Task<ServiceResultDto<CreateRoleDto>> CreateRole(CreateRoleDto role)
    {
        if(string.IsNullOrWhiteSpace(role.RoleName))
            return ServiceResultDto<CreateRoleDto>.Fail("Informe um nome válido para o papel.");

        var roleExist = await _roleManager.RoleExistsAsync(role.RoleName);
        if(roleExist)
            return ServiceResultDto<CreateRoleDto>.Fail("Já existe um papel registrado com esse nome.");

        var createRole = await _roleManager.CreateAsync(new IdentityRole { Name = role.RoleName });

        return ServiceResultDto<CreateRoleDto>.SuccessResult(role, "Papél criado com sucesso.");

    }

    public async Task<ActionResult<ServiceResultDto<IdentityRole>>> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if(role is null)
            return ServiceResultDto<IdentityRole>.Fail("Papél não encontrado.");

        var result = await _roleManager.DeleteAsync(role);
        if(!result.Succeeded)
            return ServiceResultDto<IdentityRole>.Fail("Papél não encontrado.");

        return ServiceResultDto<IdentityRole>.SuccessResult(null, "Papél deletado com sucesso.");

    }

    public async Task<ServiceResultDto<IEnumerable<RoleResponseDto>>> GetRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync( );
        var roleDtos = new List<RoleResponseDto>();
        foreach (var role in roles)
        {
            var roleDto = _mapper.Map<RoleResponseDto>(role);
            var userInRoles = await _userManager.GetUsersInRoleAsync(role.NormalizedName!);
            roleDto.totalUserInRole = userInRoles.Count;

            roleDtos.Add(roleDto);
        }

        return ServiceResultDto<IEnumerable<RoleResponseDto>>.SuccessResult(roleDtos,"Papéis listados.");
    }

}
