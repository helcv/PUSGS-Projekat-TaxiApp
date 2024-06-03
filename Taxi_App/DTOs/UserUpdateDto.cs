using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class UserUpdateDto
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Address { get; set; }
}
