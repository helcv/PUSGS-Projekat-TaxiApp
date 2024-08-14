using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Security;

namespace Taxi_App;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public MessageHub(IMessageRepository messageRepository, 
        IUserRepository userRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];

        var recipient = await _userRepository.GetUserByUsernameAsync(otherUser);
        if (recipient == null)
        {
            await Clients.Caller.SendAsync("ErrorMessage", "Recipient does not exist.");
            return;
        }

        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", _mapper.Map<IEnumerable<MessageDto>>(messages));
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await _userRepository.GetUserByUsernameAsync(Context.User.GetUsername());
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null)
        {
            throw new HubException("Recipient does not exist!");
        }

        if (sender.VerificationStatus != EVerificationStatus.ACCEPTED || sender.IsBlocked || sender.Busy)
        {
            throw new HubException("You are not allowed to send a message!");
        }

        if (recipient.VerificationStatus != EVerificationStatus.ACCEPTED || recipient.IsBlocked)
        {
            throw new HubException("Recipient does not exist!");
        }

        if (Context.User.GetUsername() == createMessageDto.RecipientUsername)
        {
            throw new HubException("You cannot send messages to yourself");
        }

        if (recipient == null)
        {
            throw new HubException("Recipient does not exist");
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
            throw new HubException("Failed to create a message.");
        }

        var group = GetGroupName(sender.UserName, recipient.UserName);
        await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
