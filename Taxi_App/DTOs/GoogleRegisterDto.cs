using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class GoogleRegisterDto
{
    [Required]
    public string GoogleToken { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public DateOnly? DateOfBirth { get; set; }
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; }
}
