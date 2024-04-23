
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Taxi_App;

public class AccountController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public AccountController(IMapper mapper, IUserRepository userRepo, ITokenService tokenService)
    {
        _mapper = mapper;
        _userRepo = userRepo;
        _tokenService = tokenService;
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

        if (await _userRepo.Register(user))
            return new UserDto{
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
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
            Token = _tokenService.CreateToken(user)
        };
    }
}