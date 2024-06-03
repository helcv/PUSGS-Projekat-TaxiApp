using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class RideService : IRideService
{

    private readonly IDistanceService _distanceService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IRideRepository _rideRepository;
    private readonly UserManager<User> _userManager;

    public RideService(IDistanceService distanceService, IUserRepository userRepository, IMapper mapper, IRideRepository rideRepository, UserManager<User> userManager)
    {
        _distanceService = distanceService;
        _userRepository = userRepository;
        _mapper = mapper;
        _rideRepository = rideRepository;
        _userManager = userManager;
    }

    public async Task<Result<SuccessMessageDto, string>> AcceptRideAsync(string username, int id)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<SuccessMessageDto, string>("You are not verified.");
        }
        if (user.IsBlocked == true) 
        {
            return Result.Failure<SuccessMessageDto, string>("You are blocked.");
        }
        if (user == null)
        {
            return Result.Failure<SuccessMessageDto, string>("User does not exist.");
        }

        var ride = await _rideRepository.GetRideById(id);
        if (ride == null)
        {
            return Result.Failure<SuccessMessageDto, string>("Ride does not exist.");
        }

        if (ride.Status != ERideStatus.CREATED) 
        {
            return Result.Failure<SuccessMessageDto, string>("Cant change ride status anymore.");
        }

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        ride.Status = ERideStatus.IN_PROGRESS;
        ride.StartTime = DateTime.UtcNow;
        ride.DriverId = user.Id;

        if(!await _rideRepository.SaveAllAsync())
        {
            return Result.Failure<SuccessMessageDto, string>("Unable to update ride status.");
        }

        return Result.Success<SuccessMessageDto, string>(new SuccessMessageDto {Message = "Ride successfully accepted."});
    }

    public async Task<Result<RideDto, string>> CreateRideAsync(string username ,AddressDto addressDto)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<RideDto, string>("You are not verified.");
        }
        if (user.IsBlocked == true) 
        {
            return Result.Failure<RideDto, string>("You are blocked.");
        }
        if (user == null)
        {
            return Result.Failure<RideDto, string>("User does not exist.");
        }

        var ride = _mapper.Map<Ride>(addressDto);
        var distanceAndDuration = await _distanceService.GetDistanceAndDuration(addressDto);

        if (distanceAndDuration.Failed)
        {
            return Result.Failure<RideDto, string>("This ride is impossible.");
        }

        Random rnd = new Random();

        ride.UserId = user.Id;
        ride.Price = _distanceService.CalculatePrice(distanceAndDuration.Distance); 
        ride.Distance = distanceAndDuration.Distance;
        ride.RideDuration = distanceAndDuration.Duration;
        ride.PickUpTime = rnd.Next(3, 21);
        ride.Status = ERideStatus.PROCESSING;

        var rideToAdd = await _rideRepository.AddRide(ride);

        if (rideToAdd == null)
        {
            return Result.Failure<RideDto, string>("Failed to create a ride.");
        }

        var rideDto = _mapper.Map<RideDto>(rideToAdd);

        return Result.Success<RideDto, string>(rideDto);
    }

    public async Task<Result<IEnumerable<CompleteRideDto>, string>> GetCompletedRidesAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<IEnumerable<CompleteRideDto>, string>("You are not verified.");
        }
        if (user.IsBlocked == true) 
        {
            return Result.Failure<IEnumerable<CompleteRideDto>, string>("You are blocked.");
        }
        if (user == null)
        {
            return Result.Failure<IEnumerable<CompleteRideDto>, string>("User does not exist.");
        }

        if (await _userManager.IsInRoleAsync(user, "User"))
        {
            var rides = await _rideRepository.GetCompletedRidesAsync(user.Id, false);
            return Result.Success<IEnumerable<CompleteRideDto>, string>(rides);
        }
        else
        {
            var rides = await _rideRepository.GetCompletedRidesAsync(user.Id, true);
            return Result.Success<IEnumerable<CompleteRideDto>, string>(rides);
        }
    }

    public async Task<Result<IEnumerable<RideDto>, string>> GetCreatedRidesAsync(string username)
    {
         var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<IEnumerable<RideDto>, string>("You are not verified.");
        }
        if (user.IsBlocked == true) 
        {
            return Result.Failure<IEnumerable<RideDto>, string>("You are blocked.");
        }
        if (user == null)
        {
            return Result.Failure<IEnumerable<RideDto>, string>("User does not exist.");
        }

        var rides = await _rideRepository.GetAllCreatedRidesAsync();
        var ridesDto = _mapper.Map<List<RideDto>>(rides);

        return Result.Success<IEnumerable<RideDto>, string>(ridesDto);
    }

    public async Task<Result<TimeDto, string>> GetRemainingTime(string username)
    {
        var ride = new Ride();
        var user = await _userManager.FindByNameAsync(username);
        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<TimeDto, string>("You are not verified.");
        }

        if (await _userManager.IsInRoleAsync(user, "Driver"))
        {
            ride = await _rideRepository.GetRideInProgressForDriverAsync(user.Id);
            if (ride == null) 
            {
                return Result.Failure<TimeDto, string>("Ride does not exist!");
            }
        }
        else
        {
            ride = await _rideRepository.GetRideInProgressForUserAsync(user.Id);
            if (ride == null) 
            {
                return Result.Failure<TimeDto, string>("Ride does not exist!");
            }
        }

        var elapsedTime = DateTime.UtcNow - ride.StartTime;
        var pickupDuration = TimeSpan.FromMinutes(ride.PickUpTime);
        var rideDuration = TimeSpan.FromMinutes(_distanceService.GetMinutes(ride.RideDuration));
        var remainingTime = pickupDuration + rideDuration - elapsedTime;
        var remainingPickupTime = pickupDuration - elapsedTime;

        var timeDto = new TimeDto();

        if (remainingPickupTime < TimeSpan.Zero)
        {
            timeDto.Message = "Driver should be on site!";
            remainingPickupTime = TimeSpan.Zero;
        }

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
            if(!await _rideRepository.CompleteRide(ride.Id))
            {
                return Result.Failure<TimeDto, string>("Something went wrong!");
            }

            timeDto.Message = "Ride should be completed. You can use app again!";
            return Result.Success<TimeDto, string>(timeDto);
        }

        timeDto.PickUpHours = remainingPickupTime.Hours;
        timeDto.PickUpMinutes = remainingPickupTime.Minutes;
        timeDto.PickUpSeconds = remainingPickupTime.Seconds;
        timeDto.RideHours = remainingTime.Hours;
        timeDto.RideMinutes = remainingTime.Minutes;
        timeDto.RideSeconds = remainingTime.Seconds;
        
        return Result.Success<TimeDto, string>(timeDto);
    }

    public async Task<Result<SuccessMessageDto, string>> RequestRideAsync(string username, int id)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<SuccessMessageDto, string>("You are not verified.");
        }
        if (user.IsBlocked == true) 
        {
            return Result.Failure<SuccessMessageDto, string>("You are blocked.");
        }
        if (user == null)
        {
            return Result.Failure<SuccessMessageDto, string>("User does not exist.");
        }

        var ride = await _rideRepository.GetRideById(id);

        if (ride == null)
        {
            return Result.Failure<SuccessMessageDto, string>("Ride does not exist.");
        }
        if (ride.UserId != user.Id) 
        {
            return Result.Failure<SuccessMessageDto, string>("You can request only your ride.");
        }
        if (ride.Status != ERideStatus.PROCESSING)
        {
            return Result.Failure<SuccessMessageDto, string>("Can not request this ride anymore.");
        }

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        ride.Status = ERideStatus.CREATED;
        if (!await _rideRepository.SaveAllAsync())
        {
            return Result.Failure<SuccessMessageDto, string>("Unable to update ride status.");
        }
        
        return Result.Success<SuccessMessageDto, string>(new SuccessMessageDto { Message = "Ride successfully requested."});
    }
}
