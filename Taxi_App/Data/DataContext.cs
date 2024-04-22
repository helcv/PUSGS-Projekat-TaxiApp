
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
}
