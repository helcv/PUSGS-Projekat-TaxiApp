using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RideController : BaseApiController
{
    private readonly IDistanceService _distanceService;

    public RideController(IDistanceService distanceService)
    {
        _distanceService = distanceService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetDistanceAndDuration(DistanceDto distanceDto)
    {
        return await _distanceService.GetDistanceAndDuration(distanceDto.From, distanceDto.To);
    }
}
