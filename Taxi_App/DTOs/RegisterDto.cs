using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class RegisterDto
{   
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "Password length must be between 6 and 15 characters")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Lastname is required")]
    public string Lastname { get; set; }
    [Required(ErrorMessage = "Date of birth is required")]
    public DateOnly? DateOfBirth { get; set; }
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
}
