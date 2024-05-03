namespace Taxi_App;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}
