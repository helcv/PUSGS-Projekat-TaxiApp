using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class RideRepository : IRideRepository
{
    private readonly DataContext _context;

    public RideRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Ride> RequestRide(int id)
    {
        try
        {
            var ride = await _context.Rides.FindAsync(id);
            ride.Status = ERideStatus.CREATED;
            await _context.SaveChangesAsync();
            return ride;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Ride> AddRide(Ride ride)
    {
        _context.Rides.Add(ride);
        await _context.SaveChangesAsync();

        return ride;
    }

    public async Task<Ride> AcceptRide(int id, int userId)
    {
        try
        {
            var ride = await _context.Rides.FindAsync(id);
            ride.Status = ERideStatus.ACCEPTED;
            ride.DriverId = userId;
            await _context.SaveChangesAsync();
            return ride;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Ride> GetRideById(int id)
    {
        return await _context.Rides.FindAsync(id);
    }

    public async Task<List<Ride>> GetRidesForUser(int id)   //add to user controller
    {
        var driverRides = await _context.Rides
            .Where(r => r.UserId == id)
            .Where(r => r.Status != ERideStatus.PROCESSING)
            .ToListAsync();

        return driverRides;
    }
}
