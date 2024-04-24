
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
        try
        {   
            _context.Users.Add(user);
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

    public async Task<User> AcceptVerification(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            user.VerificationStatus = EVerificationStatus.ACCEPTED;
            await _context.SaveChangesAsync();
            return user;
        }
        catch
        {
            return null;
        }
    }

    public async Task<User> DenyVerification(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            user.VerificationStatus = EVerificationStatus.DENIED;
            await _context.SaveChangesAsync();
            return user;
        }
        catch
        {
            return null;
        }
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}
