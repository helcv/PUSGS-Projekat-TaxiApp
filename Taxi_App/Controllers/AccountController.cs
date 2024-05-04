
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Taxi_App;

public class AccountController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AccountController(UserManager<User> userManager, RoleManager<AppRole> roleManager, IMapper mapper, IUserRepository userRepo, ITokenService tokenService, IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _userRepo = userRepo;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await _userRepo.UsernameExists(registerDto.Username.ToLower())) return BadRequest("Username is taken");

        if (await _userRepo.EmailExists(registerDto.Email)) return BadRequest("Email is taken");        // check Email and Username

        var roles = _roleManager.Roles.Select(r => r.Name.ToLower()).ToList();
        bool roleExist = await _roleManager.RoleExistsAsync(registerDto.Role.ToLower());

        if (registerDto.Role.ToLower() == "admin") return BadRequest("You can not register as admin!");     //check if role is Admin

        if (!roleExist) return BadRequest("Role does not exist!");          //check if role exist

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(registerDto, new ValidationContext(registerDto), validationResults, true);

        if (!isValid)
        {
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            return BadRequest(string.Join("; ", errorMessages));
        }

        var user = _mapper.Map<User>(registerDto);

        user.UserName = registerDto.Username.ToLower();
    
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        
        if (!result.Succeeded) return BadRequest(result.Errors);

        if (registerDto.Role.ToLower() == "driver")                                                   //check role for verification status
        {
            user.VerificationStatus = EVerificationStatus.IN_PROGRESS;
            var roleResultDriver = await _userManager.AddToRoleAsync(user, "Driver");
            if (!roleResultDriver.Succeeded) return BadRequest(roleResultDriver.Errors);
        }
        else
        {
            user.VerificationStatus = EVerificationStatus.ACCEPTED;
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
        }

        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Token = await _tokenService.CreateToken(user),
            VerificationStatus = user.VerificationStatus.ToString()
        };
        
        
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userRepo.GetUserByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid email");

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if(!result) return Unauthorized("Invalid password");

        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Token = await _tokenService.CreateToken(user),
            VerificationStatus = user.VerificationStatus.ToString()
        };
    }

    [HttpPatch("accept-verification/{id}")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(UserUpdateDto userUpdateDto)
    {
        var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        if(await _userRepo.UpdateCheckEmail(userUpdateDto.Email, user.Id)) return BadRequest("Email already exist!");
        if(await _userRepo.UpdateCheckUsername(userUpdateDto.Username, user.Id)) return BadRequest("Username already exist!");

        _mapper.Map(userUpdateDto, user);

        if (await _userRepo.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }
}