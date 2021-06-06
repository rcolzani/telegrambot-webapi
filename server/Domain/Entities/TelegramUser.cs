using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Enums;

namespace Telegram.WebAPI.Domain.Entities
{
    public class TelegramUser : Entity
    {
        public TelegramUser() { }
        public TelegramUser(long telegramChatId, TelegramUserStatus status, string name)
        {
            this.TelegramChatId = telegramChatId;
            this.Status = status;
            this.SendRiverLevel = false;
            this.Name = name;

            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
        }
        public long TelegramChatId { get; set; }
        public string Name { get; set; }
        public TelegramUserStatus Status { get; set; }
        public bool SendRiverLevel { get; set; }

        public List<Reminder> Reminders { get; set; }
        public List<MessageHistory> MessageHistory { get; set; }

        public List<Reminder> GetActiveReminders()
        {
            return Reminders.Where(r => r.Status == ReminderStatus.Activated).ToList();
        }
        public void StopReminders()
        {
            foreach (var reminder in Reminders)
            {
                reminder.StopReminder();
            }

            this.SendRiverLevel = false;
            this.Status = Domain.Enums.TelegramUserStatus.NewCliente;
            this.UpdatedAt = DateTime.Now;
        }
    }
}