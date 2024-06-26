﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public bool IsBlocked { get; set; }
    public bool Busy { get; set; }
    public double AvgRate { get; set; } = 0;
    public EVerificationStatus VerificationStatus { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<Ride> CreatedRides { get; set; } = new List<Ride>();
    public ICollection<Ride> AcceptedRides { get; set; } = new List<Ride>();
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }
}
