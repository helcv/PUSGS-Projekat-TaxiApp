using System.Text.Json.Serialization;

namespace Taxi_App;

public class Rating
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string  Message { get; set; }
    public int UserId { get; set; }
    public string UserUsername { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    public int DriverId { get; set; }
    public string DriverUsername { get; set; }
    [JsonIgnore]
    public User Driver { get; set; }
}
