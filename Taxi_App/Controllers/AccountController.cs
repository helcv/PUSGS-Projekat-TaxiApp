
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi_App;

public class AccountController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AccountController(IMapper mapper, IUserRepository userRepo, ITokenService tokenService, IEmailService emailService)
    {
        _mapper = mapper;
        _userRepo = userRepo;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await _userRepo.UsernameExists(registerDto.Username)) return BadRequest("Username is taken");

        if (await _userRepo.EmailExists(registerDto.Email)) return BadRequest("Email is taken");

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(registerDto, new ValidationContext(registerDto), validationResults, true);

         if (!isValid)
        {
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            return BadRequest(string.Join("; ", errorMessages));
        }

        var user = _mapper.Map<User>(registerDto);

        using var hmac = new HMACSHA512();

        user.Username = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;
        user.Role = (EUserType)Enum.Parse(typeof(EUserType), registerDto.Role.ToUpper());

        if (user.Role == EUserType.DRIVER)
            user.VerificationStatus = EVerificationStatus.IN_PROGRESS;
        else
            user.VerificationStatus = EVerificationStatus.ACCEPTED;

        if (await _userRepo.Register(user))
            return new UserDto{
                Username = user.Username,
                Token = _tokenService.CreateToken(user),
                VerificationStatus = user.VerificationStatus.ToString()
            };
        
        return BadRequest("Something went wrong");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userRepo.GetUserByUsername(loginDto.Username);

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if( user.PasswordHash[i] != computedHash[i])    return Unauthorized("Invalid password!");
        }

        return new UserDto
        {
            Username = user.Username,
            Token = _tokenService.CreateToken(user),
            VerificationStatus = user.VerificationStatus.ToString()
        };
    }

    [HttpPatch("accept-verification/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> AcceptVerification(int id)
    {
        var user = await _userRepo.GetUserById(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.AcceptVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }

    [HttpPatch("deny-verification/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> DenyVerification(int id)
    {
        var user = await _userRepo.GetUserById(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.DenyVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }
}