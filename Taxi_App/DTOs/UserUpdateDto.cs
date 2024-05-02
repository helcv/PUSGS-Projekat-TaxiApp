using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class UserUpdateDto
{
    public string Username { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Address { get; set; }
}
