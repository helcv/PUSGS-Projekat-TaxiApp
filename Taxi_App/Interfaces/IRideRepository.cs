using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public interface IRideRepository
{
    Task<Ride> AddRide(Ride ride);
    Task<Ride> GetRideById(int id);
    Task<Ride> GetCreatedRideForUser(int id);
    Task<List<Ride>> GetAllRidesAsync();
    Task<List<Ride>> GetAllCreatedRidesAsync();
    Task<List<CompleteRideDto>> GetCompletedRidesAsync(int id, bool isDriver);
    Task<Ride> GetRideInProgressForUserAsync(int userId);
    Task<Ride> GetRideInProgressForDriverAsync(int userId);
    Task<bool> CompleteRide(int id);
    Task DeleteRideAsync(int rideId);
    Task<bool> SaveAllAsync();
}
