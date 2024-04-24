namespace Taxi_App;

public interface IEmailService
{
    Task SendEmail(string email, string verification);
}
