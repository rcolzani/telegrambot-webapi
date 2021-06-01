using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Enums;

namespace Telegram.WebAPI.Domain.Entities
{
    public class TelegramUser : Entity
    {
        public TelegramUser() { }
        public TelegramUser(long telegramChatId, TelegramUserStatus status)
        {
            this.TelegramChatId = telegramChatId;
            this.Status = status;
            this.SendRiverLevel = false;

            this.CreatedAt = DateTime.Now;
        }
        public long TelegramChatId { get; set; }
        public string Name { get; set; }
        public TelegramUserStatus Status { get; set; }

        public bool SendRiverLevel { get; set; }

        public virtual List<Reminder> Reminders { get; set; }
        public virtual List<MessageHistory> MessageHistory { get; set; }
    }
}