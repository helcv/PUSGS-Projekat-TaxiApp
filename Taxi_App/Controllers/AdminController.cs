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
        var drivers = await _userManager.GetUsersInRoleAsync("Driver");
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
}
