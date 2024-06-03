using CSharpFunctionalExtensions;

namespace Taxi_App;

public interface IRatingService
{
    Task<Result<SuccessMessageDto, string>> CreateRateAsync(string username, CreateRateDto createRateDto);
}
