using AutoMapper;
using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelBookingAPI.Controllers;

[Authorize(Roles = "Admin, Employee")]
public class AccountController: Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IAccount _account;
    private readonly IMapper _mapper;
    private readonly IUserVerifierService _userRoleVerifier;

    public AccountController
        (
        IMapper mapper,
        IAccount account,
        IConfiguration configuration,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserVerifierService userRoleVerifier
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _account = account;
        _mapper = mapper;
        _userRoleVerifier = userRoleVerifier;
    }
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]UserRegisterDto userRegister)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _account.Register(userRegister);

        if(!result.Success)
            return BadRequest(new { result.Message,result.Errors });

        return Ok( new {result.Message, Sucess = result.Success });
        
    }
    
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDto loginDto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _account.Login(loginDto);
        if(!result.Success)
        {
            if(result.Message == "Usuário não encontrado.")
                return NotFound(new { message = result.Message, success = result.Success} );
            if(result.Message == "Senha inválida.")
                return Unauthorized(new {message = result.Message, success = result.Success});

            return BadRequest(new { message = result.Message, Success = result.Success});
        }
        var token = await GenerateToken(result.Data!);

        return Ok( new { token, message = result.Message, success = result.Success });
    }

    [HttpGet("detail")]
    public async Task<ActionResult<ServiceResultDto<UserDetailDto>>> GetUserDetail()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString();
        if(currentUserId is null)
            return NotFound(ServiceResultDto<UserDetailDto>.Fail("Usuário não encontrado."));

        var user = await _userManager.FindByIdAsync(currentUserId!);
        var result = await _account.GetUserDetail(user!);


        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<ActionResult<ServiceResultDto<IEnumerable<UserDetailDto>>>> GetUsers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString();
        if(await _userRoleVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _account.GetUsers( );
        return Ok( result );
    }

    private async Task<string> GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler( );

        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSetting").GetSection("SecurityKey").Value!);

        var roles = await _userManager.GetRolesAsync(user);

        List<Claim> claims = new( )
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}" ?? ""),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? ""),
            new Claim(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JwtSetting").GetSection("ValidAudience").Value! ?? ""),
            new Claim(JwtRegisteredClaimNames.Iss,_configuration.GetSection("JwtSetting").GetSection("ValidIssuer").Value! ?? "")
        };
        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role,role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
