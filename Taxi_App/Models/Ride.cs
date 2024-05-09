namespace Taxi_App;

public class Ride
{
    public int Id { get; set; }
    public string StartAddress { get; set; }
    public string FinalAddress { get; set; }
    public string Distance { get; set; }
    public float Price { get; set; }
    public int PickUpTime { get; set; }
    public string RideDuration { get; set; }
    public DateTime StartTime { get; set; }
    public ERideStatus Status { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public int? DriverId { get; set; }
    public User Driver { get; set; }
}
