using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RideController : BaseApiController
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IRideService _rideService;

    public RideController(IRideService rideService, IHttpContextAccessor httpContextAccessor)
    {
        _rideService = rideService;
        _contextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RideDto>> CreateRide(AddressDto addressDto)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.CreateRideAsync(currUsername, addressDto);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/request-ride")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> RequestRide(int id)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.RequestRideAsync(currUsername, id);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/deny-ride")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> DenyRide(int id)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.DenyRideRequestAsync(currUsername, id);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/accept-ride")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult> AcceptRide(int id)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.AcceptRideAsync(currUsername, id);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("created-rides")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<List<RideDto>>> GetAllCreatedRides()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.GetCreatedRidesAsync(currUsername);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("completed-rides")]
    [Authorize(Roles = "Driver, User")]
    public async Task<ActionResult<List<CompleteRideDto>>> GetCompletedRides()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.GetCompletedRidesAsync(currUsername);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("in-progress")]
    [Authorize(Roles = "Driver, User")]
    public async Task<IActionResult> GetRideInProgress()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currId = currUser.GetUserId();

        var result = await _rideService.GetRideInProgressAsync(currId);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("created")]
    [Authorize(Roles = " User")]
    public async Task<IActionResult> GetCreatedRideForUser()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currId = currUser.GetUserId();

        var result = await _rideService.GetCreatedRideAsync(currId);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("remaining-time")]
    [Authorize]
    public async Task<ActionResult<TimeDto>> GetRemainingTime()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _rideService.GetRemainingTime(currUsername);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
