using CSharpFunctionalExtensions;

namespace Taxi_App;

public interface IRideService
{
    Task<Result<RideDto, string>> CreateRideAsync(string username, AddressDto addressDto);
    Task<Result<SuccessMessageDto, string>> RequestRideAsync(string username, int id);
    Task<Result<SuccessMessageDto, string>> AcceptRideAsync(string username, int id);
    Task<Result<IEnumerable<RideDto>, string>> GetCreatedRidesAsync(string username);
    Task<Result<IEnumerable<CompleteRideDto>, string>> GetCompletedRidesAsync(string username);
    Task<Result<TimeDto, string>> GetRemainingTime(string username);
    Task<Result<SuccessMessageDto, string>> DenyRideRequestAsync(string username, int id);
     Task<Result<RideDto, string>> GetCreatedRideAsync(int userId);
}
