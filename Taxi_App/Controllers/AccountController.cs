
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi_App;

public class AccountController : BaseApiController
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _contextAccessor;

    public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currId = currUser.GetUserId();

        var result = await _accountService.GetProfileAsync(currId);
        if (result.IsFailure) return BadRequest(result.Error);
        
        return Ok(result.Value);

    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfileById(int id)
    {
        var result = await _accountService.GetProfileAsync(id);
        if (result.IsFailure) return BadRequest(result.Error);
        
        return Ok(result.Value);

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(registerDto, new ValidationContext(registerDto), validationResults, true);

        if (!isValid)
        {
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            return BadRequest(string.Join("; ", errorMessages));
        }

        var result = await _accountService.RegisterAsync(registerDto);
        if (result.IsFailure) return BadRequest(result.Error);
        
        return Ok(result.Value);
    }

    [HttpPost("signin-google")]
    public async Task<IActionResult> GoogleRegister(GoogleRegisterDto googleRegisterDto)
    {
        var result = await _accountService.GoogleRegisterAsync(googleRegisterDto);
        if (result.IsFailure) return BadRequest(result.Error);
        
        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var result = await _accountService.Login(loginDto);
        if (result.IsFailure) return Unauthorized(result.Error);

        return Ok(result.Value);
    }


    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(UserUpdateDto userUpdateDto)
    {
        var currUser = _contextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _accountService.UpdateAsync(currUsername, userUpdateDto);
        if (result.IsFailure) return BadRequest(result.Error);
        
        return Ok(result.Value);
    }
}