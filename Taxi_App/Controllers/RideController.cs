using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RideController : BaseApiController
{
    private readonly IDistanceService _distanceService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IRideRepository _rideRepository;

    public RideController(IDistanceService distanceService, IUserRepository userRepository, IMapper mapper, IRideRepository rideRepository)
    {
        _distanceService = distanceService;
        _userRepository = userRepository;
        _mapper = mapper;
        _rideRepository = rideRepository;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetDistance(DistanceDto distanceDto)
    {
        var data = await _distanceService.GetDistanceAndDuration(distanceDto.StartAddress, distanceDto.FinalAddress);
        return data[0];
    }

    [HttpPost]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<RideDto>> CreateRide(DistanceDto distanceDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        var ride = _mapper.Map<Ride>(distanceDto);
        var data = await _distanceService.GetDistanceAndDuration(distanceDto.StartAddress, distanceDto.FinalAddress);

        if (data[0] == "ZERO_RESULTS" || data[0] == "NOT_FOUND") return BadRequest("This ride is impossible!");

        Random rnd = new Random();

        ride.UserId = user.Id;
        ride.Price = _distanceService.CalculatePrice(data[0]); 
        ride.Distance = data[0];
        ride.RideDuration = data[1];
        ride.PickUpTime = rnd.Next(3, 21);
        ride.Status = ERideStatus.PROCESSING;

        var rideToAdd = await _rideRepository.AddRide(ride);

        if (rideToAdd == null) return BadRequest("Failed to add ride");

        var rideDto = _mapper.Map<RideDto>(rideToAdd);

        return Ok(rideDto);
    }

    [HttpPatch("request-ride/{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult> RequestRide(int id)
    {
        var ride = await _rideRepository.GetRideById(id);

        if (ride == null) return NotFound("Ride doen't exist!");

        if (ride.Status != ERideStatus.PROCESSING) return BadRequest("Can't request this ride anymore!");
        var rideAccepted = await _rideRepository.RequestRide(ride.Id);
        var rideDto = _mapper.Map<RideDto>(rideAccepted);
        return Ok(rideDto);
    }

    [HttpPatch("accept-ride/{id}")]
    [Authorize(Roles = "Driver, Admin")]
    public async Task<ActionResult> AcceptRide(int id)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");

        var ride = await _rideRepository.GetRideById(id);

        if (ride == null) return NotFound("Ride doen't exist!");

        if (ride.Status != ERideStatus.CREATED) return BadRequest("Can't change ride status anymore!");

        var rideAccepted = await _rideRepository.AcceptRide(ride.Id, user.Id);
        var rideDto = _mapper.Map<RideDto>(rideAccepted);
        return Ok(rideDto);
    }
}
