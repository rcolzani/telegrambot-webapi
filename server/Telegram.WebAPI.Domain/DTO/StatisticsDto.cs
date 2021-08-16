using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.WebAPI.Domain.DTO
{
   public class StatisticsDto
    {
        public StatisticsDto(int activeUsersQuantity, int usersQuantity, int messagesReceivedQuantity, int messagesSentQuantity)
        {
            ActiveUsersQuantity = activeUsersQuantity;
            UsersQuantity = usersQuantity;
            MessagesReceivedQuantity = messagesReceivedQuantity;
            MessagesSentQuantity = messagesSentQuantity;
        }

        public int ActiveUsersQuantity { get; private set; }
        public int UsersQuantity { get; private set; }
        public int MessagesReceivedQuantity { get; private set; }
        public int MessagesSentQuantity { get; private set; }
    }
}
