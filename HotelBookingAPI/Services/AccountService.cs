using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace HotelBookingAPI.Services;

public class AccountService: IAccount
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IUserVerifierService _userRoleVerifier;

    public AccountService(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserVerifierService roleVerifier)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _userRoleVerifier = roleVerifier;
    }

    public async Task<ServiceResultDto<UserDetailDto>> GetUserDetail(AppUser user)
    {
        var userDto = _mapper.Map<UserDetailDto>(user);
        //var userDto = new UserDetailDto
        //{
        //    Id = user!.Id,
        //    FirstName = user!.FirstName,
        //    LastName = user.LastName,
        //    Email = user.Email,
        //    Roles = [.. await _userManager.GetRolesAsync(user)],
        //    PhoneNumber = user.PhoneNumber,
        //    TwoFactorEnabled = user.TwoFactorEnabled,
        //    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
        //    AcessFailedCount = user.AccessFailedCount,
        //};
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
        var user = new AppUser
        {
            Email = userRegisterDto.EmailAddress,
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
            UserName = userRegisterDto.EmailAddress,
            
        };

        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);
        var errors = result.Errors.Select(err => err.Description);

        if (!result.Succeeded)       
            return ServiceResultDto<AppUser>.Fail("Falha ao registrar usuário", errors);

        if (userRegisterDto.Roles is null)
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
}
