using CSharpFunctionalExtensions;
using Taxi_App.DTOs;

namespace Taxi_App;

public interface IAccountService
{
    Task<Result<SuccessCreateDto, IEnumerable<string>>> RegisterAsync(RegisterDto registerDto);
    Task<Result<SuccessCreateDto, IEnumerable<string>>> GoogleRegisterAsync(GoogleRegisterDto googleRegisterDto);
    Task<Result<TokenDto, string>> Login(LoginDto loginDto);
    Task<Result<TokenDto, string>> GoogleLogin(string googleToken);
    Task<Result<SuccessMessageDto, IEnumerable<string>>> UpdateAsync(string username, UserUpdateDto userUpdateDto);
    Task<Result<SuccessMessageDto, IEnumerable<string>>> UpdatePasswordAsync(string username, PasswordUpdateDto passwordUpdateDto);
    Task<Result<UserDto, string>> GetProfileAsync(int id);
}
