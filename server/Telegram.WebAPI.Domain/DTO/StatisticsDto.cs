using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.WebAPI.Domain.DTO
{
   public class StatisticsDto
    {
        public int ActiveUsersQuantity { get; set; }
        public int UsersQuantity { get; set; }
        public int MessagesReceivedQuantity { get; set; }
        public int MessagesSentQuantity { get; set; }
    }
}
