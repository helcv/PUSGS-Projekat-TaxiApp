using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public interface IRideRepository
{
    Task<Ride> AddRide(Ride ride);
    Task<Ride> RequestRide(int id);
    Task<Ride> AcceptRide(int id, int userId);
    Task<Ride> GetRideById(int id);
    Task<List<Ride>> GetRidesForUser(int id);
}
