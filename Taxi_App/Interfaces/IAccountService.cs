using CSharpFunctionalExtensions;

namespace Taxi_App;

public interface IAccountService
{
    Task<Result<SuccessCreateDto, IEnumerable<string>>> RegisterAsync(RegisterDto registerDto);
    Task<Result<SuccessCreateDto, IEnumerable<string>>> GoogleRegisterAsync(GoogleRegisterDto googleRegisterDto);
    Task<Result<TokenDto, string>> Login(LoginDto loginDto);
    Task<Result<SuccessMessageDto, IEnumerable<string>>> UpdateAsync(string username, UserUpdateDto userUpdateDto);
    Task<Result<UserDto, string>> GetProfileAsync(int id);
}
