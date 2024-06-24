using AutoMapper;
using CSharpFunctionalExtensions;

namespace Taxi_App;

public class MessageService : IMessageService
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageService(IUserRepository userRepository,
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<MessageDto, string>> CreateMessageAsync(string username, CreateMessageDto createMessageDto)
    {
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if(recipient == null)
        {
            return Result.Failure<MessageDto, string>("Recipient does not exist!");
        }

        if(sender.VerificationStatus != EVerificationStatus.ACCEPTED || sender.IsBlocked || sender.Busy)
        {
            return Result.Failure<MessageDto, string>("You are not allowed to send a message!");
        }

        if(recipient.VerificationStatus != EVerificationStatus.ACCEPTED || recipient.IsBlocked)
        {
            
        }

        if(username == createMessageDto.RecipientUsername)
        {
            return Result.Failure<MessageDto, string>("You cannot send messages to yourself");
        }

        if (recipient == null)
        {
            return Result.Failure<MessageDto, string>("Recipient does not exist");
        }

        var message = new Message 
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _messageRepository.AddMessage(message);
        if (!await _messageRepository.SaveAllAsync())
        {
            return Result.Failure<MessageDto, string>("Failed to create a message.");
        }

        return Result.Success<MessageDto,string>(_mapper.Map<MessageDto>(message));
    }

    public async Task<Result<IEnumerable<MessageDto>, string>> GetMessagesForUserAsync(string container, string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if(user.Busy || user.IsBlocked || user.VerificationStatus != EVerificationStatus.ACCEPTED)
        {
            return Result.Failure<IEnumerable<MessageDto>, string>("Not allowed!");
        }

        var messages = await _messageRepository.GetMessagesForUserAsync(container, username);

        return Result.Success<IEnumerable<MessageDto>, string>(messages);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currUsername, string recipientUsername)
    {
        var messages = await _messageRepository.GetMessageThreadAsync(currUsername, recipientUsername);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<Result<SuccessMessageDto, string>> DeleteMessageAsync(int id, string username)
    {
        var message = await _messageRepository.GetMessageAsync(id);

        if (message.SenderUsername != username && message.RecipientUsername != username)
        {
            return Result.Failure<SuccessMessageDto, string>("Not allowed!");
        }

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.RecipientDeleted && message.SenderDeleted)
        {
            _messageRepository.DeleteMessage(message);
        }

        if(!await _messageRepository.SaveAllAsync())
        {
            return Result.Failure<SuccessMessageDto, string>("Failed to save changes!");
        }

        return Result.Success<SuccessMessageDto,string>(new SuccessMessageDto{Message = "Message successfully deleted."});
    }
}
