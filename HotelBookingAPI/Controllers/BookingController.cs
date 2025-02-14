using HotelBookingAPI.Dtos;
using HotelBookingAPI.Enums;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BookingController: ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IBooking _bookingService;
    private readonly IUserVerifier _userVerifier;

    public BookingController(AppDbContext dbContext,IBooking bookingService,IUserVerifier userVerifier)
    {
        _dbContext = dbContext;
        _bookingService = bookingService;
        _userVerifier = userVerifier;
    }
    [HttpPost]
    public async Task<ActionResult<ServiceResultDto<CreateBookingDto>>> CreateBooking([FromBody] BookingRequest bookingRequest)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        var traveler = await _dbContext.Travelers!.FirstOrDefaultAsync(t => t.UserId == currentUserId);
        if(traveler is null)
            return Unauthorized(ServiceResultDto<List<TravelerDetailDto>>.Fail("Viajante não autênticado."));

        var result = await _bookingService.CreateBooking(bookingRequest);
        if(!result.Success)
            return BadRequest(result.Errors);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResultDto<List<BookingDto>>>> GetAllBookings()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _bookingService.GetAllBookings( );
        if(!result.Success)
            return NotFound( );

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResultDto<BookingDto>>> GetBooking(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _bookingService.GetBooking(id);
        if(!result.Success)
            return NotFound( );

        return Ok(result);
    }

    [HttpGet("by-nationalID/{travelerNationalId}")]
    public async Task<ActionResult<ServiceResultDto<BookingDto>>> GetBookingByTravelerNationalId(string travelerNationalId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var result = await _bookingService.GetBookingByTravelerNationalId(travelerNationalId);
        if(!result.Success)
            return NotFound(ServiceResultDto<BookingDto>.Fail(result.Message));

        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ServiceResultDto<string>>> UpdateBookingStatus(int id, BookingStatus bookingStatus)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var updateStatus = await _bookingService.UpdateBookingStatus(id,currentUserId!,bookingStatus);
        if(!updateStatus.Success)
            return BadRequest( );

        return Ok(updateStatus);
    }

    [HttpGet("{bookingStatus:alpha}")]
    public async Task<ActionResult<ServiceResultDto<List<BookingDto>>>> GetBookingByStatus(BookingStatus bookingStatus)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<IEnumerable<UserDetailDto>>.Fail("Usuário não autênticado."));

        var bookingsByStatus = await _bookingService.GetBookingsByStatus(bookingStatus);
        if(!bookingsByStatus.Success)
            return BadRequest(ServiceResultDto<List<BookingDto>>.Fail(bookingsByStatus.Message));

        return Ok(bookingsByStatus);
    }

    [HttpPatch("checkin/{bookingId}")]
    public async Task<ActionResult<ServiceResultDto<string>>> Checkin(int bookingId, bool confirmAction = false)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<string>.Fail("Usuário não autênticado."));

        var result = await _bookingService.Checkin(bookingId, confirmAction);
        if(!result.Success)
            return Forbid(ServiceResultDto<string>.Fail("Erro ao realizar check-in.", result.Errors).Message);

        return Ok(ServiceResultDto<string>.SuccessResult("Checkin realizado.",""));
    }

    [HttpPatch("checkout/{bookingId}")]
    public async Task<ActionResult<ServiceResultDto<string>>> Checkout(int bookingId,bool confirmAction = false)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString( );
        if(await _userVerifier.VerifyUserEmployeeOrAdminOrNull(currentUserId!) == false)
            return Unauthorized(ServiceResultDto<string>.Fail("Usuário não autênticado."));

        var result = await _bookingService.Checkout(bookingId,confirmAction);
        if(!result.Success)
            return BadRequest(ServiceResultDto<string>.Fail("Erro ao realizar check-out.",result.Errors));

        return Ok(ServiceResultDto<string>.SuccessResult("Checkout realizado.",""));
    }
}
