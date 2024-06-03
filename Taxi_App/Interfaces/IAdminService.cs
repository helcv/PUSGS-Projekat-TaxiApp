using CSharpFunctionalExtensions;

namespace Taxi_App;

public interface IAdminService
{
    Task<Result<SuccessMessageDto, string>> VerificationAsync(int id, bool shouldAccept);
    Task<Result<SuccessMessageDto, string>> ChangeBlockingStatusAsync(int id, bool shouldBlock);
    Task<IEnumerable<DriverDto>> GetAllDriversAsync();
    Task<IEnumerable<RideDto>> GetAllRidesAsync();
}
