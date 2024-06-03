
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm]RegisterDto registerDto)
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