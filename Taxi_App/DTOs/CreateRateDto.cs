using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class CreateRateDto
{
    [Required]
    public string DriverUsername { get; set; }
    [Required]
    [Range(1,5, ErrorMessage = "Rate must be between 1 and 5")]
    public int Stars { get; set; }
    [Required]
    public string Message { get; set; }
}

