namespace Taxi_App;

public interface IDistanceService
{
    Task<string> GetDistanceAndDuration(string from, string to);
}
