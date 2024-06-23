using CSharpFunctionalExtensions;

namespace Taxi_App;

public interface IMessageService
{
    Task<Result<MessageDto, string>> CreateMessageAsync(string username, CreateMessageDto createMessageDto);
    Task<Result<IEnumerable<MessageDto>, string>> GetMessagesForUserAsync(string username, string container);
    Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currUsername, string recipientUsername);
}
