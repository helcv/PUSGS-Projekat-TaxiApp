using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class RatingService : IRatingService
{

    private readonly IRatingRepository _ratingRepository;
    private readonly IRideRepository _rideRepository;
    private readonly UserManager<User>  _userManager;

    public RatingService(IRatingRepository ratingRepository, IRideRepository rideRepository, UserManager<User> userManager)
    {
        _userManager = userManager;
        _ratingRepository = ratingRepository;
        _rideRepository = rideRepository;
    }

    public async Task<Result<SuccessMessageDto, string>> CreateRateAsync(string username, CreateRateDto createRateDto)
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

        var completedRides = await _rideRepository.GetCompletedRidesAsync(user.Id, false);
        if (completedRides.Count() == 0) 
        {
            return Result.Failure<SuccessMessageDto, string>("There are currently no completed rides.");
        }

        foreach (var completedRide in completedRides)
        {
            if (completedRide.Username == createRateDto.DriverUsername)
            {
                var driver = await _userManager.FindByNameAsync(createRateDto.DriverUsername);
                if (driver == null)
                {
                    return Result.Failure<SuccessMessageDto, string>("Driver does not exist.");
                }

                var rating = new Rating() 
                {
                    Stars = createRateDto.Stars,
                    Message = createRateDto.Message,
                    User = user,
                    Driver = driver,
                    UserUsername = user.UserName,
                    DriverUsername = createRateDto.DriverUsername
                };

                _ratingRepository.AddRating(rating);
                if (!await _ratingRepository.SaveAllAsync()) 
                {
                    return Result.Failure<SuccessMessageDto, string>("Failed to create a rate.");
                }

                return Result.Success<SuccessMessageDto, string>(new SuccessMessageDto { Message = "Rate successfully created."});
            }
        }
        return Result.Failure<SuccessMessageDto, string>("You can not rate this user.");
    }
}
