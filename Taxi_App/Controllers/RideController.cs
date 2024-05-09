using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RideController : BaseApiController
{
    private readonly IDistanceService _distanceService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IRideRepository _rideRepository;
    private readonly UserManager<User> _userManager;

    public RideController(IDistanceService distanceService, IUserRepository userRepository, IMapper mapper, IRideRepository rideRepository, UserManager<User> userManager)
    {
        _distanceService = distanceService;
        _userRepository = userRepository;
        _mapper = mapper;
        _rideRepository = rideRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetDistance(AddressDto addressDto)
    {
        var distanceAndDuration = await _distanceService.GetDistanceAndDuration(addressDto);
        return distanceAndDuration.Distance;
    }

    [HttpPost]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<RideDto>> CreateRide(AddressDto addressDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user.IsBlocked == true) return Unauthorized("You are blocked.");

        var ride = _mapper.Map<Ride>(addressDto);
        var distanceAndDuration = await _distanceService.GetDistanceAndDuration(addressDto);

        if (distanceAndDuration.Failed == true) return BadRequest("This ride is impossible!");

        Random rnd = new Random();

        ride.UserId = user.Id;
        ride.Price = _distanceService.CalculatePrice(distanceAndDuration.Distance); 
        ride.Distance = distanceAndDuration.Distance;
        ride.RideDuration = distanceAndDuration.Duration;
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
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");
        if (user.IsBlocked == true) return Unauthorized("You are blocked.");

        var ride = await _rideRepository.GetRideById(id);

        if (ride == null) return NotFound("Ride doen't exist!");

        if (ride.Status != ERideStatus.PROCESSING) return BadRequest("Can't request this ride anymore!");

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        var rideAccepted = await _rideRepository.RequestRide(ride.Id);
        var rideDto = _mapper.Map<RideDto>(rideAccepted);
        return Ok(rideDto);
    }

    [HttpPatch("accept-ride/{id}")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult> AcceptRide(int id)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user.IsBlocked == true) return Unauthorized("You are blocked.");
        if (user == null) return NotFound();
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");

        var ride = await _rideRepository.GetRideById(id);

        if (ride == null) return NotFound("Ride doen't exist!");

        if (ride.Status != ERideStatus.CREATED) return BadRequest("Can't change ride status anymore!");

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        var rideAccepted = await _rideRepository.AcceptRide(ride.Id, user.Id);
        var rideDto = _mapper.Map<RideDto>(rideAccepted);
        return Ok(rideDto);
    }

    [HttpGet("created-rides")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<List<RideDto>>> GetAllCreatedRides()
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");
        if (user.IsBlocked == true) return Unauthorized("You are blocked.");

        var rides = await _rideRepository.GetAllCreatedRidesAsync();
        if (rides.Count() == 0) return NotFound("There are currently no created rides");

        var ridesDto = _mapper.Map<List<RideDto>>(rides);

        return Ok(ridesDto);
    }

    [HttpGet("completed-rides")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<List<RideDto>>> GetCompletedRides()
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");
        if (user.IsBlocked == true) return Unauthorized("You are blocked.");

        var rides = await _rideRepository.GetAllCreatedRidesAsync();
        if (rides.Count() == 0) return NotFound("There are currently completed no rides");

        var ridesDto = _mapper.Map<List<RideDto>>(rides);

        return Ok(ridesDto);
    }

    [HttpGet("remaining-time")]
    [Authorize]
    public async Task<ActionResult<string>> GetRemainingTime()
    {
        //check if user is allowed
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");
        if (user.IsBlocked == false) return Ok("Ride should be completed.\n You can use app again!");
        var ride = new Ride();

        //get specific ride
        if (await _userManager.IsInRoleAsync(user, "Driver"))
        {
            ride = await _rideRepository.GetRideInProgressForDriverAsync(user.Id);
            if (ride == null) return NotFound("Ride does not exist!");
        }
        else
        {
            ride = await _rideRepository.GetRideInProgressForUserAsync(user.Id);
            if (ride == null) return NotFound("Ride does not exist!");
        }

        //count duration for pickup and drop off
        var elapsedTime = DateTime.UtcNow - ride.StartTime;
        var pickupDuration = TimeSpan.FromMinutes(ride.PickUpTime);
        var rideDuration = TimeSpan.FromMinutes(_distanceService.GetMinutes(ride.RideDuration));
        var remainingTime = pickupDuration + rideDuration - elapsedTime;
        var remainingPickupTime = pickupDuration - elapsedTime; 
        
        var retPickup = $"Pickup duration is approximately {remainingPickupTime.Hours} hours, {remainingPickupTime.Minutes } minutes, {remainingPickupTime.Seconds} seconds.";
        var retRide = $"Ride duration is approximately {remainingTime.Hours} hours, {remainingTime.Minutes } minutes, {remainingTime.Seconds} seconds.";

        if (remainingPickupTime < TimeSpan.Zero)
            retPickup = "Driver should be on site!\n";

        if (remainingTime < TimeSpan.Zero)
        {
            var currUser = await _userRepository.GetUserByIdAsync(ride.UserId);
            var currDriver = new User();

            //when times up allow user and driver to use app
            if(ride.DriverId.HasValue)
                currDriver = await _userRepository.GetUserByIdAsync(ride.DriverId ?? 0);
            else
                currDriver = null;

            currUser.IsBlocked = false;
            currDriver.IsBlocked = false;

            await _userManager.UpdateAsync(currUser);
            await _userManager.UpdateAsync(currDriver);

            //and mark ride as completed
            if(await _rideRepository.CompleteRide(ride.Id) == false) return BadRequest("Something went wrong.");

            return Ok("Ride should be completed.\n You can use app again!");
        }

        return Ok(retPickup + retRide);
    }
}
