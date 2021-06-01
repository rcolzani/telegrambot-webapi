using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Entities
{
    public class Reminder : Entity
    {
        public string TextMessage { get; set; }
        public TimeSpan RemindTimeToSend { get; set; }
        public DateTime RemindedAt { get; set; }
        public bool Active { get; set; }

        public int TelegramUserId { get; set; }
        public virtual TelegramUser TelegramUser { get; set; }
    }
}
