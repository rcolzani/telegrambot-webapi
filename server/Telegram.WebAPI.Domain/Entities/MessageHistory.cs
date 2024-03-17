using System;

namespace Telegram.WebAPI.Domain.Entities;

public class MessageHistory(Guid userId, string textMessage, DateTime messageDate, bool messageSent)
{
    public string TextMessage => textMessage;
    public DateTime MessageDate => messageDate;
    public bool MessageSent => messageSent;
    public Guid UserId => userId;
}

