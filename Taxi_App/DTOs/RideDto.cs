namespace Taxi_App;

public class RideDto
{
    public string StartAddress { get; set; }
    public string FinalAddress { get; set; }
    public float Price { get; set; }
    public int PickUpTime { get; set; }
    public string Distance { get; set; }
    public string RideDuration { get; set; }
    public string Status { get; set; }
}
