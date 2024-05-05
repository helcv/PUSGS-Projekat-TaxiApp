using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;

namespace Taxi_App;

public class DistanceService : IDistanceService
{
    private readonly IConfiguration _config;

    public DistanceService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<string>> GetDistanceAndDuration(string from, string to)
    {
        var baseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json?";

        string[] partsFrom = from.Split(',');
        string[] partsTo = to.Split(',');
        
        string streetNumFrom = partsFrom[0].Trim();
        string streetFrom = partsFrom[1].Trim();
        string cityFrom = partsFrom[2].Trim();
        string countryFrom = partsFrom[3].Trim();

        string streetNumTo = partsTo[0].Trim();
        string streetTo = partsTo[1].Trim();
        string cityTo = partsTo[2].Trim();
        string countryTo = partsTo[3].Trim();
    
        using (HttpClient client = new HttpClient())
        {
            var queryParams = new Dictionary<string, string>();

            queryParams.Add("key", $"{_config["APIKey"]}");
            queryParams.Add("origins", $"{streetNumFrom}, {streetFrom}, {cityFrom}, {countryFrom}");
            queryParams.Add("mode", "driving");
            queryParams.Add("language", "en-En");
            queryParams.Add("destinations", $"{streetNumTo}, {streetTo}, {cityTo}, {countryTo}");

            var newUrl = new Uri(QueryHelpers.AddQueryString(baseUrl, queryParams));

            using (HttpResponseMessage response = await client.GetAsync(newUrl))
                {
                    //catch the results from the api
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var retList = new List<string>();

                    JObject parsedData = JObject.Parse(responseContent);

                    string err = (string)parsedData["rows"][0]["elements"][0]["status"];

                    if (err == "ZERO_RESULTS" || err == "NOT_FOUND")
                    {
                        retList.Add(err);
                        return retList;
                    }

                    string distanceText = (string)parsedData["rows"][0]["elements"][0]["distance"]["text"];
                    string durationText = (string)parsedData["rows"][0]["elements"][0]["duration"]["text"];

                    retList.Add(distanceText);
                    retList.Add(durationText);

                    return retList;
                }
        }
    } 

    public float CalculatePrice(string distance)
    {
        var distanceParts = distance.Split(' ');
        float distanceKm = float.Parse(distanceParts[0]);
        float totalPrice = (float)Math.Round(distanceKm * 0.75, 3);

        return totalPrice;  //price in €
    }
}
