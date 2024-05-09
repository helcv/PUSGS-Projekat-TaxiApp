using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public interface IRideRepository
{
    Task<Ride> AddRide(Ride ride);
    Task<Ride> RequestRide(int id);
    Task<Ride> AcceptRide(int id, int userId);
    Task<Ride> GetRideById(int id);
    Task<List<Ride>> GetRidesForUser(int id);
    Task<List<Ride>> GetAllRidesAsync();
    Task<List<Ride>> GetAllCreatedRidesAsync();
    Task<List<Ride>> GetCompletedRidesAsync(int id);
    Task<Ride> GetRideInProgressForUserAsync(int userId);
    Task<Ride> GetRideInProgressForDriverAsync(int userId);
    Task<bool> CompleteRide(int id);
}
