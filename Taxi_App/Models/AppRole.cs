using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}
