namespace Taxi_App;

public interface IUserRepository
{
    Task<bool> UsernameExists(string username); 
    Task<bool> EmailExists(string email);
    Task<bool> UpdateCheckUsername(string username, int currentId); 
    Task<bool> UpdateCheckEmail(string email, int currentId); 
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(int id);
    void Update(User user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<User>> GetDriversWithRates();
}
