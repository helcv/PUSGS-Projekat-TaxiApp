using AutoMapper;
using CSharpFunctionalExtensions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Taxi_App.DTOs;

namespace Taxi_App;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;
    private readonly IConfigurationSection _googleCredentials;
    private readonly IUserRepository _userRepo;

    public AccountService(UserManager<User> userManager,
        IMapper mapper, ITokenService tokenService,
        IConfiguration config, IPhotoService photoService, IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _photoService = photoService;
        _googleCredentials = config.GetSection("GoogleClientId");
    }

    public async Task<Result<SuccessCreateDto, IEnumerable<string>>> GoogleRegisterAsync(GoogleRegisterDto googleRegisterDto)
    {
        var errMessages = new List<string>();
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleRegisterDto.GoogleToken);

        if (payload.Audience.ToString() != _googleCredentials.Value){
            throw new InvalidJwtException("Invalid token");
        }

        if (googleRegisterDto.Role != Roles.User.ToString() && googleRegisterDto.Role != Roles.Driver.ToString())
        {
            errMessages.Add("Role does not exist!");
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        var userToRegister = _mapper.Map<User>(googleRegisterDto);
        string[] parts = payload.Email.Split('@');
        
        userToRegister.UserName = parts[0];
        userToRegister.Email = payload.Email;
        userToRegister.PhotoUrl = payload.Picture;
        userToRegister.Name = payload.GivenName;
        userToRegister.Lastname = payload.FamilyName;

        var result = await _userManager.CreateAsync(userToRegister, googleRegisterDto.Password);
        if (!result.Succeeded) 
        {
            errMessages.AddRange(result.Errors.Select(error => error.Description));
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        if (googleRegisterDto.Role.ToLower() == "driver")
        {
            userToRegister.VerificationStatus = EVerificationStatus.IN_PROGRESS;
            var roleResultDriver = await _userManager.AddToRoleAsync(userToRegister, "Driver");
            if (!roleResultDriver.Succeeded) 
            {
                errMessages.Add("Role does not exist");
                return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages); 
            }
        }
        else
        {
            userToRegister.VerificationStatus = EVerificationStatus.ACCEPTED;
            var roleResult = await _userManager.AddToRoleAsync(userToRegister, "User");
            if (!roleResult.Succeeded)
            {
                errMessages.Add("Role does not exist");
                return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages); 
            }
        }

        var successCreateDto = new SuccessCreateDto
        {
            Id = userToRegister.Id,
            Token = await _tokenService.CreateToken(userToRegister)
        };

        return Result.Success<SuccessCreateDto, IEnumerable<string>>(successCreateDto);
    }

    public async Task<Result<TokenDto, string>> GoogleLogin(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

        if (payload.Audience.ToString() != _googleCredentials.Value){
            throw new InvalidJwtException("Invalid token");
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            return Result.Failure<TokenDto, string>("Invalid email address.");
        }
        if (user.VerificationStatus == EVerificationStatus.DENIED)
        {
            return Result.Failure<TokenDto, string>("Verification denied.");
        }

        var token = new TokenDto
        {
            Token = await _tokenService.CreateToken(user)
        };

        return Result.Success<TokenDto, string>(token);
    }

    public async Task<Result<TokenDto, string>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Result.Failure<TokenDto, string>("Invalid email address.");
        }
        if (user.VerificationStatus == EVerificationStatus.DENIED)
        {
            return Result.Failure<TokenDto, string>("Verification denied.");
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
        {
            return Result.Failure<TokenDto, string>("Invalid password.");
        }

        var token = new TokenDto
        {
            Token = await _tokenService.CreateToken(user)
        };

        return Result.Success<TokenDto, string>(token);
    }

    public async Task<Result<SuccessCreateDto, IEnumerable<string>>> RegisterAsync(RegisterDto registerDto)
    {
        var errMessages = new List<string>();
        var userToRegister = _mapper.Map<User>(registerDto);

        if (registerDto.DateOfBirth.Value.Year > 2008 || registerDto.DateOfBirth.Value.Year < 1900)
        {
            errMessages.Add("You must be 16+");
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }
        if (registerDto.Role != Roles.User.ToString() && registerDto.Role != Roles.Driver.ToString())
        {
            errMessages.Add("Role does not exist!");
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        var result = await _userManager.CreateAsync(userToRegister, registerDto.Password);
        if (!result.Succeeded) 
        {
            errMessages.AddRange(result.Errors.Select(error => error.Description));
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        if (registerDto.Role.ToLower() == "driver")
        {
            userToRegister.VerificationStatus = EVerificationStatus.IN_PROGRESS;
            var roleResultDriver = await _userManager.AddToRoleAsync(userToRegister, "Driver");
            if (!roleResultDriver.Succeeded) 
            {
                errMessages.Add("Role does not exist");
                return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages); 
            }
        }
        else
        {
            userToRegister.VerificationStatus = EVerificationStatus.ACCEPTED;
            var roleResult = await _userManager.AddToRoleAsync(userToRegister, "User");
            if (!roleResult.Succeeded)
            {
                errMessages.Add("Role does not exist");
                return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages); 
            }
        }

        var photoResult = await _photoService.AddPhotoAsync(registerDto.Photo);
        if (photoResult.Error != null) 
        {
            errMessages.Add(photoResult.Error.Message);
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        userToRegister.PhotoUrl = photoResult.SecureUrl.AbsoluteUri;
        var setPhotoResult = await _userManager.UpdateAsync(userToRegister);
        if (!result.Succeeded)
        {
            errMessages.AddRange(result.Errors.Select(error => error.Description));
            return Result.Failure<SuccessCreateDto, IEnumerable<string>>(errMessages);
        }

        var successCreateDto = new SuccessCreateDto
        {
            Id = userToRegister.Id,
            Token = await _tokenService.CreateToken(userToRegister)
        };
        
        return Result.Success<SuccessCreateDto, IEnumerable<string>>(successCreateDto);
    }

    public async Task<Result<SuccessMessageDto, IEnumerable<string>>> UpdateAsync(string username, UserUpdateDto userUpdateDto)
    {
        var errMessages = new List<string>();
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            errMessages.Add("You are not verified");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }
        if (user.IsBlocked == true)
        {
            errMessages.Add("You are blocked");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }
        if (user == null) 
        {
            errMessages.Add("User does not exist");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }

        user.Name = userUpdateDto.Name;
        user.Lastname = userUpdateDto.Lastname;
        user.Address = userUpdateDto.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            errMessages.AddRange(result.Errors.Select(error => error.Description));
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }

        return Result.Success<SuccessMessageDto, IEnumerable<string>>(new SuccessMessageDto { Message = "User verification successfully updated."});
    }

     public async Task<Result<SuccessMessageDto, IEnumerable<string>>> UpdatePasswordAsync(string username, PasswordUpdateDto passwordUpdateDto)
     {
        var errMessages = new List<string>();
        var user = await _userManager.FindByNameAsync(username);

        if (user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            errMessages.Add("You are not verified");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }
        if (user.IsBlocked == true)
        {
            errMessages.Add("You are blocked");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }
        if (user == null) 
        {
            errMessages.Add("User does not exist");
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }

        var updateResult = await _userManager.ChangePasswordAsync(user, passwordUpdateDto.OldPassword, passwordUpdateDto.NewPassword);
        if (!updateResult.Succeeded)
        {
            errMessages.AddRange(updateResult.Errors.Select(error => error.Description));
            return Result.Failure<SuccessMessageDto, IEnumerable<string>>(errMessages);
        }

        return Result.Success<SuccessMessageDto, IEnumerable<string>>(new SuccessMessageDto { Message = "User password successfully updated!"} );
     }

    public async Task<Result<UserDto, string>> GetProfileAsync(int id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user.VerificationStatus == EVerificationStatus.DENIED)
        {
            return Result.Failure<UserDto, string>("You are not allowed");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.UserName,
            Lastname = user.Lastname,
            Email = user.Email,
            PhotoUrl = user.PhotoUrl,
            Age = user.DateOfBirth.CalculateAge(),
            Address = user.Address,
            VerificationStatus = user.VerificationStatus.ToString(),
            Busy = user.Busy,
            IsBlocked = user.IsBlocked
        };

        return Result.Success<UserDto, string>(userDto);
    }
}
