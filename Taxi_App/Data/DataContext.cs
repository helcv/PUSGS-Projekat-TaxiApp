
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class DataContext : IdentityDbContext<User, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {
        
    }
    //public DbSet<User> Users { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        
        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<User>()
            .HasMany(r => r.CreatedRides)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .IsRequired();

        builder.Entity<Ride>()
            .HasOne(u => u.User)
            .WithMany(r => r.CreatedRides)
            .HasForeignKey(r => r.UserId)
            .IsRequired();

        builder.Entity<User>()
            .HasMany(r => r.Ratings)
            .WithOne(r => r.Driver)
            .HasForeignKey(r => r.DriverId)
            .IsRequired();
        
        builder.Entity<Rating>()
            .HasOne(u => u.Driver)
            .WithMany(r => r.Ratings)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
