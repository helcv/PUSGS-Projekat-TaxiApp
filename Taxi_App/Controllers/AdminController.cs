using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IRideRepository _rideRepository;

    public AdminController(IUserRepository userRepo, IEmailService emailService, IMapper mapper, UserManager<User> userManager, IRideRepository rideRepository)
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _mapper = mapper;
        _userManager = userManager;
        _rideRepository = rideRepository;
    }

    [HttpPatch("accept-verification/{id}")]
    public async Task<ActionResult> AcceptVerification(int id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.AcceptVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }

    [HttpPatch("deny-verification/{id}")]
    public async Task<ActionResult> DenyVerification(int id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.DenyVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }

    [HttpGet("drivers")]
    public async Task<ActionResult<List<DriverDto>>> GetAllDrivers()
    {
        var drivers = await _userRepo.GetDriversWithRates();
        if (drivers.Count() == 0) return NotFound("There are currently no drivers!");

        var driversDto = _mapper.Map<List<DriverDto>>(drivers);

        return Ok(driversDto);
    }

    [HttpGet("rides")]
    public async Task<ActionResult<List<RideDto>>> GetAllRides()
    {
        var rides = await _rideRepository.GetAllRidesAsync();
        if (rides.Count() == 0) return NotFound("There are currently no rides!");

        var ridesDto = _mapper.Map<List<RideDto>>(rides);

        return Ok(ridesDto);
    }

    [HttpPatch("block-driver/{username}")]
    public async Task<ActionResult<BlockDriverDto>> BlockDriver(string username)
    {
        var driver = await _userRepo.GetUserByUsernameAsync(username);

        if (driver == null) return NotFound("Driver does not exist!");
        if (driver.VerificationStatus == EVerificationStatus.IN_PROGRESS) return NotFound("Can not block this driver!");
        if (driver.IsBlocked == true) return BadRequest("Driver is already blocked!");

        driver = await _userRepo.BlockDriverAsync(username);
        if (driver == null) return BadRequest("Something went wrong");
        
        return Ok(_mapper.Map<BlockDriverDto>(driver));
    }

    [HttpPatch("unblock-driver/{username}")]
    public async Task<ActionResult<BlockDriverDto>> UnblockDriver(string username)
    {
        var driver = await _userRepo.GetUserByUsernameAsync(username);

        if (driver == null) return NotFound("Driver does not exist!");
        if (driver.VerificationStatus == EVerificationStatus.IN_PROGRESS) return NotFound("Can not unblock this driver!");
        if (driver.IsBlocked == false) return BadRequest("Driver is already unblocked!");

        driver = await _userRepo.UnblockDriverAsync(username);
        if (driver == null) return BadRequest("Something went wrong");
        
        return Ok(_mapper.Map<BlockDriverDto>(driver));
    }
}
