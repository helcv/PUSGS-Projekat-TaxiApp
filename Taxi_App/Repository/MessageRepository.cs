
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Taxi_App;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<List<MessageDto>> GetMessagesForUserAsync(string container, string username)
    {
        var query = _context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = container switch
        {
            "Inbox" => query.Where(u => u.RecipientUsername == username && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.SenderUsername == username && u.SenderDeleted == false),
            _ => query.Where(u => u.RecipientUsername == username && u.RecipientDeleted == false && u.DateRead == null)
        };
        

        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetMessageThreadAsync(string currUserUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .Where(
                m => m.RecipientUsername == currUserUsername &&
                m.RecipientDeleted == false &&
                m.SenderUsername == recipientUsername || 
                m.RecipientUsername == recipientUsername &&
                m.SenderDeleted == false &&
                m.SenderUsername == currUserUsername
            )
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currUserUsername).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return messages;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
