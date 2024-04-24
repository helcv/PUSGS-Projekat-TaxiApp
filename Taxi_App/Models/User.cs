using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public EVerificationStatus VerificationStatus { get; set; }
    public EUserType Role { get; set; }
}
