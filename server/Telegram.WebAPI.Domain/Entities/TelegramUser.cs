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
            this.SendRiverLevel = false;
            this.Name = name;

            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;

            this.Reminders = new List<Reminder>();
            this.MessageHistory = new List<MessageHistory>();
        }
        public long TelegramChatId { get; private set; }
        public string Name { get; private set; }
        public bool SendRiverLevel { get; private set; }

        public List<Reminder> Reminders { get; set; }
        public List<MessageHistory> MessageHistory { get; set; }

        public List<Reminder> GetActiveReminders()
        {
            return Reminders.Where(r => r.Status == ReminderStatus.Activated).ToList();
        }
        public void StartReceiveRiverLevel()
        {
            this.SendRiverLevel = true;
        }
        public void StopReceiveRiverLevel()
        {
            this.SendRiverLevel = false;
        }

        public void StopReminders()
        {
            if (Reminders == null)
            {
                return;
            }

            foreach (var reminder in Reminders)
            {
                reminder.StopReminder();
            }

            this.SendRiverLevel = false;
            this.UpdatedAt = DateTime.Now;
        }
    }
}