using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Taxi_App;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IRideRepository _rideRepository;

    public AdminService(IUserRepository userRepo, IEmailService emailService, IMapper mapper, IRideRepository rideRepository)
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _mapper = mapper;
        _rideRepository = rideRepository;
    }

    public async Task<Result<SuccessMessageDto, string>> ChangeBlockingStatusAsync(int id, bool shouldBlock)
    {
        var driver = await _userRepo.GetUserByIdAsync(id);

        if (driver == null)
        {
            return Result.Failure<SuccessMessageDto, string>("Driver does not exist.");
        }
        if (driver.VerificationStatus == EVerificationStatus.IN_PROGRESS)
        {
            return Result.Failure<SuccessMessageDto, string>("Can not block this driver.");
        }
        if (driver.IsBlocked && shouldBlock)
        {
            return Result.Failure<SuccessMessageDto, string>("Driver is already blocked.");
        }
        if (!driver.IsBlocked && !shouldBlock)
        {
            return Result.Failure<SuccessMessageDto, string>("Driver is already unblocked.");
        }

        if (!shouldBlock)
            driver.IsBlocked = false;
        else
            driver.IsBlocked = true;

        if(!await _userRepo.SaveAllAsync())
        {
            return Result.Failure<SuccessMessageDto, string>("Failed to change block status.");
        }
         
         return Result.Success<SuccessMessageDto, string>(new SuccessMessageDto { Message = "Block status changed successfully."});
    }

    public async Task<IEnumerable<DriverDto>> GetAllDriversAsync()
    {
        var drivers = await _userRepo.GetDriversWithRates();

        foreach (var driver in drivers)
        {
            if (driver.Ratings != null && driver.Ratings.Any())
            {
                driver.AvgRate = driver.Ratings.Average(d => d.Stars);
            }
            else
            {
                driver.AvgRate = 0;
            }
        }

        var driversDto = _mapper.Map<List<DriverDto>>(drivers);

        return driversDto;
    }

    public async Task<IEnumerable<RideDto>> GetAllRidesAsync()
    {
        var rides = await _rideRepository.GetAllRidesAsync();

        var ridesDto = _mapper.Map<List<RideDto>>(rides);

        return ridesDto;
    }

    public async Task<Result<SuccessMessageDto, string>> VerificationAsync(int id, bool shouldAccept)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user == null)
        {
            return Result.Failure<SuccessMessageDto, string>("User does not exist.");
        }

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS)
        {
            return Result.Failure<SuccessMessageDto, string>("Can't change verification anymore!");
        }

        if (!shouldAccept)
            user.VerificationStatus = EVerificationStatus.DENIED;
        else
            user.VerificationStatus = EVerificationStatus.ACCEPTED;

        if(!await _userRepo.SaveAllAsync())
        {
            return Result.Failure<SuccessMessageDto, string>("Failed to verify user.");
        }
        
        await _emailService.SendEmail(user.Email, user.VerificationStatus.ToString());

        return Result.Success<SuccessMessageDto, string>(new SuccessMessageDto { Message = "User successfully verified." });
    }
}
