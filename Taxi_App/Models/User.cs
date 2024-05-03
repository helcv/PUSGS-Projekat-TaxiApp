using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public EVerificationStatus VerificationStatus { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }
}
