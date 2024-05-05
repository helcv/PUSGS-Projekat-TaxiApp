namespace Taxi_App;

public interface IDistanceService
{
    Task<List<string>> GetDistanceAndDuration(string from, string to);
    float CalculatePrice(string distance);
}
