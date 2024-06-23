namespace Taxi_App;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<List<MessageDto>> GetMessagesForUserAsync(string container, string username);
    Task<IEnumerable<Message>> GetMessageThreadAsync(string currUserUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
}
