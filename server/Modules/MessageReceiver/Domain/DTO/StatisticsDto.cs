namespace Telegram.WebAPI.Domain.DTO;

public class StatisticsDto(int activeUsersQuantity, int usersQuantity, int messagesReceivedQuantity, int messagesSentQuantity)
{
    public int ActiveUsersQuantity => activeUsersQuantity;
    public int UsersQuantity => usersQuantity;
    public int MessagesReceivedQuantity => messagesReceivedQuantity;
    public int MessagesSentQuantity => messagesSentQuantity;
}
