using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RatingController : BaseApiController
{
    private readonly IRatingService _ratingService;
    private readonly IHttpContextAccessor _contextAccessor;

    public RatingController(IRatingService ratingService, IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
        _ratingService = ratingService;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RatingDto>> RateDriver(CreateRateDto createRateDto)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _ratingService.CreateRateAsync(currUsername, createRateDto);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
