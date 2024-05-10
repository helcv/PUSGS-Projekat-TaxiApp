namespace Taxi_App;

public interface IRatingRepository
{
    void AddRating(Rating rating);
    Task<bool> SaveAllAsync();
}
