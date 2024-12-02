namespace HotelBookingAPI.Infra.Data.Repositories;

public interface IUserVerifierService
{
    Task<bool> VerifyUserEmployeeOrAdminOrNull(string id);
}
