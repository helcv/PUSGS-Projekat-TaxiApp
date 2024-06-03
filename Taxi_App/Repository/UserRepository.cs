
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;

    public UserRepository(DataContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> UsernameExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username);
    }
    public async Task<bool> EmailExists(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public void Update(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCheckUsername(string username, int currentId)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username && u.Id != currentId);
    }

    public async Task<bool> UpdateCheckEmail(string email, int currentId)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && u.Id != currentId);
    }

    public async Task<IEnumerable<User>> GetDriversWithRates()
    {
        var drivers = await _userManager.GetUsersInRoleAsync("Driver");

        var driversWithRates = await _context.Users
            .Where(u => drivers.Contains(u))
            .Where(u => u.VerificationStatus != EVerificationStatus.IN_PROGRESS)
            .Include(u => u.Ratings)
            .ToListAsync();

        return driversWithRates;
    }
}
