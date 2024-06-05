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

    public async Task<bool> CompleteRide(int id)
    {
        try
        {
            var ride = await _context.Rides.FindAsync(id);
            ride.Status = ERideStatus.COMPLETED;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Ride> GetRideInProgressForUserAsync(int userId)
    {
        return await _context.Rides.FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ERideStatus.IN_PROGRESS);
    }

    public async Task<Ride> GetRideInProgressForDriverAsync(int userId)
    {
        return await _context.Rides.FirstOrDefaultAsync(r => r.DriverId == userId && r.Status == ERideStatus.IN_PROGRESS);
    }

    public async Task<List<CompleteRideDto>> GetCompletedRidesAsync(int id, bool isDriver)
{
    IQueryable<Ride> ridesQuery;

    if (isDriver)
    {
        ridesQuery = _context.Rides.Where(r => r.Status == ERideStatus.COMPLETED && r.DriverId == id);
    }
    else
    {
        ridesQuery = _context.Rides.Where(r => r.Status == ERideStatus.COMPLETED && r.UserId == id);
    }

    return await ridesQuery.Select(r => new CompleteRideDto
    {
        Id = r.Id,
        StartAddress = r.StartAddress,
        FinalAddress = r.FinalAddress,
        Price = r.Price,
        PickUpTime = r.PickUpTime,
        Distance = r.Distance,
        RideDuration = r.RideDuration,
        Status = "Completed",
        Username = isDriver ? r.User.UserName : r.Driver.UserName
    }).ToListAsync();
}
    
    public async Task<List<Ride>> GetAllCreatedRidesAsync()
    {
        return await _context.Rides.Where(r => r.Status == ERideStatus.CREATED).ToListAsync();
    }

    public async Task<List<Ride>> GetAllRidesAsync()
    {
        return  await _context.Rides
            .Include(u => u.User)
            .Include(d => d.Driver)
            .ToListAsync();
    }

    public async Task<Ride> AddRide(Ride ride)
    {
        _context.Rides.Add(ride);
        await _context.SaveChangesAsync();

        return ride;
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

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
