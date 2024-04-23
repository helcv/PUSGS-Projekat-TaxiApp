namespace Taxi_App;

public interface IUserRepository
{
    Task<bool> Register(User user);
    Task<bool> UsernameExists(string username); 
    Task<bool> EmailExists(string email); 
    Task<User> GetUserByUsername(string username);
}
