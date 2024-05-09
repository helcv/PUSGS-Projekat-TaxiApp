namespace Taxi_App;

public interface IDistanceService
{
    Task<DistanceDto> GetDistanceAndDuration(AddressDto addressDto);
    float CalculatePrice(string distance);
    int GetMinutes(string duration);
}
