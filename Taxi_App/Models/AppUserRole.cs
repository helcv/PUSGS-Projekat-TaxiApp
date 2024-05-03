using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class AppUserRole : IdentityUserRole<int>
{
    public User User { get; set; }
    public AppRole Role { get; set; }
}
