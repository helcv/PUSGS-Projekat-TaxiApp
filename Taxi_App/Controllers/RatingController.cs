using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class RatingController : BaseApiController
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RatingController(IRatingRepository ratingRepository, IRideRepository rideRepository, IUserRepository userRepository, IMapper mapper)
    {
        _ratingRepository = ratingRepository;
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RatingDto>> RateDriver(CreateRateDto createRateDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED) return Unauthorized("You are not verified!");
        if (user.IsBlocked == true) return Unauthorized("You are blocked.");
        if (user == null) return NotFound("User does not exist!");

        var completedRides = await _rideRepository.GetCompletedRidesForUserAsync(user.Id);
        if (completedRides.Count() == 0) return Unauthorized("There are currently no completed rides!");

        foreach (var completedRide in completedRides)
        {
            if (completedRide.Username == createRateDto.DriverUsername)
            {
                if (createRateDto.Stars < 1 || createRateDto.Stars > 5) return BadRequest("Rating must be between 1 and 5!");

                var driver = await _userRepository.GetUserByUsernameAsync(createRateDto.DriverUsername);
                if (driver == null) return NotFound("Driver does not exist!");

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
                if (await _ratingRepository.SaveAllAsync() ) return Ok(_mapper.Map<RatingDto>(rating));
            }
        }
        return Unauthorized("You can not rate this user!");
    }
}
