namespace Taxi_App;

public class DriverDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string VerificationStatus { get; set; }
    public double AvgRate { get; set; }
    public bool IsBlocked { get; set; }
    public ICollection<RatingDto> Ratings { get; set; }
}
