using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HotelBookingAPI.Services;

public class AccountService : IAccount
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IUserVerifier _userVerifier;

    public AccountService(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserVerifier roleVerifier)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _userVerifier = roleVerifier;
    }

    public async Task<ServiceResultDto<UserDetailDto>> GetUserDetail(AppUser user)
    {
        var userDto = _mapper.Map<UserDetailDto>(user);
        userDto.Roles = [.. await _userManager.GetRolesAsync(user)];
        

        var userFinded = ServiceResultDto<UserDetailDto>.SuccessResult( userDto,"Usuário encontrado.");
       
        return userFinded;
    }

    public async Task<ServiceResultDto<IEnumerable<UserDetailDto>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync( );
        var usersDetail = new List<UserDetailDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersDetail.Add(_mapper.Map<UserDetailDto>(user));
        }
        var ok = ServiceResultDto<IEnumerable<UserDetailDto>>.SuccessResult(usersDetail,"Usuários localizados.");
        return ok;
    }

    public async Task<ServiceResultDto<AppUser>> Login(UserLoginDto userLoginDto)
    {
        var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
        if(user == null)
            return ServiceResultDto<AppUser>.Fail("Usuário não encontrado.");
        

        var isValidPassword = await _userManager.CheckPasswordAsync(user,userLoginDto.Password);
        if(!isValidPassword)
            return ServiceResultDto<AppUser>.Fail("Senha inválida.");
        

        return ServiceResultDto<AppUser>.SuccessResult(user,"Usuário logado com sucesso.");
    }

    public async Task<ServiceResultDto<AppUser>> Register(UserRegisterDto userRegisterDto)
    {
        var validationResult = _userVerifier.ValidateUser(userRegisterDto);
        if(!validationResult.Success)
            return validationResult;

        var isDuplicateUser = await _userVerifier.IsDuplicateUser(userRegisterDto);

        if(!isDuplicateUser.Success)
            return ServiceResultDto<AppUser>.Fail("Documento duplicado.", new List<string> { isDuplicateUser.Message });

        var user = _mapper.Map<AppUser>(userRegisterDto);

        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);
        var errors = result.Errors.Select(err => err.Description);

        if (!result.Succeeded)       
            return ServiceResultDto<AppUser>.Fail("Falha ao registrar usuário", errors);

        if (userRegisterDto.Roles is null || !userRegisterDto.Roles.Any())
        {
            await _userManager.AddToRoleAsync(user,"User");
        }
        else
        {
            foreach (var role in userRegisterDto.Roles)
            {
                await _userManager.AddToRoleAsync(user,role);
            }
        }

        return ServiceResultDto<AppUser>.SuccessResult(user,"Usuário criado com sucesso");
    }

    public async Task<ServiceResultDto<AppUser>> UpdateUser(string id, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user == null)
            return ServiceResultDto<AppUser>.Fail("Usuário não encontrado.");

        var validationResult = _userVerifier.ValidateUser(updateUserDto);
        if(!validationResult.Success)
            return validationResult;

        if(user.NationalId != updateUserDto.NationalId || user.RegistrationId != updateUserDto.RegistrationId)
        {
            var isDuplicateUser = await _userVerifier.IsDuplicateUser(updateUserDto);

            if(!isDuplicateUser.Success)
                return ServiceResultDto<AppUser>.Fail("Documento duplicado.", new[] { isDuplicateUser.Message });
        }
        else
        {
            updateUserDto.NationalId = user.NationalId;
            updateUserDto.RegistrationId = user.RegistrationId;
        }

        var userUpdated = _mapper.Map(updateUserDto, user);
        userUpdated.EditedBy = user.Id;
        

        var result = await _userManager.UpdateAsync(user);
        var errors = result.Errors.Select(err => err.Description);

        if(!result.Succeeded)
            return ServiceResultDto<AppUser>.Fail("Falha ao editar usuário",errors);

        return ServiceResultDto<AppUser>.SuccessResult(user,"Usuário editado com sucesso.");
    }
}
