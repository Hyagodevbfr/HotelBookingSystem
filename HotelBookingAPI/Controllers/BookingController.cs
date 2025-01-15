using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
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

    public BookingController(AppDbContext dbContext, IBooking bookingService)
    {
        _dbContext = dbContext;
        _bookingService = bookingService;
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
}
