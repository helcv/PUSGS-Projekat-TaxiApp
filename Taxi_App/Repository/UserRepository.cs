
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> Register(User user)
    {
        _context.Users.Add(user);

        try
        {   
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UsernameExists(string username)
    {
        return await _context.Users.AnyAsync(x=> x.Username == username);
    }
    public async Task<bool> EmailExists(string email)
    {
        return await _context.Users.AnyAsync(x=> x.Email == email);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
    }
}
