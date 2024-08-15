using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Security;
using Taxi_App.Models;

namespace Taxi_App;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IMessageRepository messageRepository, 
        IUserRepository userRepository,
        IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];

        var recipient = await _userRepository.GetUserByUsernameAsync(otherUser);
        if (recipient == null || recipient.UserName == Context.User.GetUsername())
        {
            await Clients.Caller.SendAsync("ErrorMessage", "Recipient does not exist.");
            return;
        }

        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await _messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", _mapper.Map<IEnumerable<MessageDto>>(messages));
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
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

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _messageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connection = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connection != null)
            {
                await _presenceHub.Clients.Clients(connection)
                    .SendAsync("NewMessageReceived", sender.UserName);
            }
        }

        _messageRepository.AddMessage(message);
        if (!await _messageRepository.SaveAllAsync())
        {
            throw new HubException("Failed to create a message.");
        }

        await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _messageRepository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
    }

}
